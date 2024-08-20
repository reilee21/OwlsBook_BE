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
    public class ReviewController : ControllerBase
    {
        private readonly Owls_BookContext context;

        public ReviewController(Owls_BookContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ReviewFilter filter)
        {
            var query = context.Reviews
                                .Include(r => r.Book).ThenInclude(b => b.BookImages)
                                .Include(r => r.Customer)
                                .OrderByDescending(r => r.CreateAt)
                                .AsQueryable();
            if (!string.IsNullOrEmpty(filter.SearchName))
            {
                query = query.Where(r => r.Book.BookTitle.ToLower().Contains(filter.SearchName.ToLower().Trim()));
            }
            var totalItems = await query.CountAsync();
            var reviews = await query
               .Skip(((int)filter.Page - 1) * (int)filter.PageSize)
               .Take((int)filter.PageSize)
               .ToListAsync();


            var result = new PageList<Review>
            {
                PageIndex = (int)filter.Page,
                PageSize = (int)filter.PageSize,
                TotalItems = totalItems,
                Items = reviews,
                TotalPages = (int)Math.Ceiling((totalItems / (double)filter.PageSize))
            };

            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update(ReviewUpdate update)
        {
            var rv = await context.Reviews.FindAsync(update.ReviewId);
            if (rv == null) { return NotFound(); }
            rv.Status = update.Status;
            context.Entry(rv).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(rv);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var rv = await context.Reviews.FindAsync(id);
            if (rv == null) { return NotFound(); }
            context.Entry(rv).State = EntityState.Deleted;
            await context.SaveChangesAsync();
            return Ok(rv);
        }

    }
}
