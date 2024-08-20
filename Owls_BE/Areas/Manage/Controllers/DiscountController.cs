using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;

        public DiscountController(Owls_BookContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterRequest filter)
        {
            var discounts = await context.Discounts.ToListAsync();
            if (!discounts.Any())
            { Ok(); }
            var disc = discounts.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
            PageList<Discount> rs = new PageList<Discount>
            {
                PageIndex = (int)filter.Page,
                PageSize = (int)filter.PageSize,
                TotalItems = discounts.Count(),
                Items = disc,
            };
            rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));
            return Ok(rs);
        }
        [HttpGet]

        public async Task<IActionResult> GetByName([FromQuery] string? name)
        {
            if (string.IsNullOrEmpty(name))
                return Ok();
            string cleanText = name.Trim().ToLower();
            var rs = await context.Discounts.Where(d => d.DiscountName.ToLower().Contains(cleanText) && d.Active == true).ToListAsync();
            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> Create(DiscountCreate discount)
        {
            if (ModelState.IsValid)
            {
                if (discount.StartDate.HasValue && discount.EndDate.HasValue)
                {
                    bool check = ValidDateRange((DateTime)discount.StartDate, (DateTime)discount.EndDate);
                    if (!check)
                        return BadRequest("Invalid date range");
                }
                var newdiscount = mapper.Map<Discount>(discount);
                newdiscount.Active = false;
                try
                {
                    context.Discounts.Add(newdiscount);
                    await context.SaveChangesAsync();
                    return Ok(newdiscount);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return BadRequest("Invalid data format");
        }
        [HttpPut]
        public async Task<IActionResult> Edit(DiscountEdit updateDiscount)
        {
            if (ModelState.IsValid)
            {
                var existingDiscount = await context.Discounts.FirstOrDefaultAsync(vc => vc.DiscountId == updateDiscount.DiscountId);
                if (existingDiscount == null)
                {
                    return NotFound();
                }
                if (updateDiscount.StartDate.HasValue && updateDiscount.EndDate.HasValue)
                {
                    bool check = ValidDateRange((DateTime)updateDiscount.StartDate, (DateTime)updateDiscount.EndDate);
                    if (!check)
                        return BadRequest("Invalid date range");

                }

                mapper.Map(updateDiscount, existingDiscount);

                try
                {
                    context.Entry(existingDiscount).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok(existingDiscount);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return BadRequest("Invalid data format");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var discount = await context.Discounts.FindAsync(id);
            if (discount == null)
                return NotFound();
            try
            {
                context.Entry(discount).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(discount);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyOldBook(DiscountOldBook disc)
        {
            var discount = await context.Discounts.FindAsync(disc.DiscountId);
            if (discount == null)
                return NotFound();

            var bks = await context.Books
                                   .Include(b => b.BookImages)
                                   .Include(b => b.Discounts)
                                   .Where(b => b.IsActive == true)
                                   .OrderBy(b => b.PublishedYear)
                                   .Take(10)
                                   .ToListAsync();
            foreach (var b in bks)
            {
                if (!b.Discounts.Any(d => d.DiscountId.Equals(discount.DiscountId)))
                {
                    b.Discounts.Add(discount);
                }
            }
            context.Books.UpdateRange(bks);
            await context.SaveChangesAsync();

            return Ok();
        }
        private bool ValidDateRange(DateTime s, DateTime e)
        {
            return e >= s;
        }
    }
}
