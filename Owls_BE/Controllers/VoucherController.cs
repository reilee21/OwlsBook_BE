using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Repositories.VoucherRepos;
using System.Security.Claims;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepos voucherRepos;
        private readonly IMapper mapper;

        public VoucherController(IVoucherRepos voucherRepos, IMapper mapper)
        {
            this.voucherRepos = voucherRepos;
            this.mapper = mapper;
        }


        [HttpGet("{voucherId}")]
        public async Task<IActionResult> Getvoucher(VoucherApply voucher)
        {
            var d = await voucherRepos.GetVoucher(voucher.Code);
            if (d == null) return NotFound();
            var rs = mapper.Map<VoucherRead>(d);
            return Ok(rs);
        }

        [HttpPost]
        public async Task<IActionResult> CheckVoucher(VoucherApply voucher)
        {
            if (voucher == null || string.IsNullOrEmpty(voucher.Code)) return BadRequest(voucher);
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            string rs = await voucherRepos.CheckApplyVoucher(voucher, username);
            if (rs.Equals("quantity")) return Ok(new ErrorStatus { Code = "002", Message = "Voucher đã đủ số lượng" });
            if (rs.Equals("amount")) return Ok(new ErrorStatus { Code = "003", Message = "Đơn hàng chưa đủ giá trị tổi thiểu" });
            if (string.IsNullOrEmpty(rs)) return Ok(new ErrorStatus { Code = "004", Message = "Mã " + voucher.Code + " không chính xác" });

            var vc = await voucherRepos.GetVoucher(voucher.Code);
            var vcher = mapper.Map<VoucherRead>(vc);

            return Ok(vcher);
        }

    }
}
