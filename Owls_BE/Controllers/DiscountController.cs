using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Response;
using Owls_BE.Repositories.DiscountRepos;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepos discountRepos;
        private readonly IMapper mapper;

        public DiscountController(IDiscountRepos discountRepos, IMapper mapper)
        {
            this.discountRepos = discountRepos;
            this.mapper = mapper;
        }


        [HttpGet("{discountId}")]
        public async Task<IActionResult> GetDiscount(string discountId)
        {
            var d = await discountRepos.GetDiscountById(discountId);
            if (d == null) return NotFound();
            var rs = mapper.Map<DiscountRead>(d);
            return Ok(rs);
        }

    }
}
