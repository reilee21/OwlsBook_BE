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
    public class VoucherController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;

        public VoucherController(Owls_BookContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterRequest filter)
        {
            var vouchers = await context.Vouchers.ToListAsync();
            if (!vouchers.Any()) { Ok(); }
            var vcrs = vouchers.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
            PageList<Voucher> rs = new PageList<Voucher>
            {
                PageIndex = (int)filter.Page,
                PageSize = (int)filter.PageSize,
                TotalItems = vouchers.Count(),
                Items = vcrs,
            };
            rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));

            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> Create(VoucherCreate newVoucher)
        {
            if (ModelState.IsValid)
            {
                var voucher = mapper.Map<Voucher>(newVoucher);
                voucher.Code = voucher.Code.ToUpper();
                voucher.Active = false;
                if (await CheckCodeExist(voucher.Code)) return Ok(new ErrorStatus { Code = "11", Message = "CODE đã được sử dụng" });
                try
                {
                    context.Vouchers.Add(voucher);
                    await context.SaveChangesAsync();
                    return Ok(voucher);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return BadRequest("Invalid data format");

        }
        [HttpPut]
        public async Task<IActionResult> Edit(VoucherEdit updatedVoucher)
        {
            if (ModelState.IsValid)
            {
                var existingVoucher = await context.Vouchers.FirstOrDefaultAsync(vc => vc.VoucherId == updatedVoucher.VoucherId);
                if (existingVoucher == null)
                {
                    return NotFound();
                }

                var voucherWithSameCode = await context.Vouchers.FirstOrDefaultAsync(vc => vc.Code == updatedVoucher.Code.Normalize());
                if (voucherWithSameCode != null && voucherWithSameCode.VoucherId != updatedVoucher.VoucherId)
                {
                    return Ok(new ErrorStatus { Code = "11", Message = "CODE đã được sử dụng" });
                }

                mapper.Map(updatedVoucher, existingVoucher);
                existingVoucher.Code = existingVoucher.Code.ToUpper();

                try
                {
                    context.Entry(existingVoucher).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok(existingVoucher);
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
            var voucher = await context.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();
            try
            {
                context.Entry(voucher).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(voucher);
        }
        private async Task<bool> CheckCodeExist(string code)
        {
            var vch = await context.Vouchers.FirstOrDefaultAsync(x => x.Code == code);
            return vch != null;
        }
    }
}
