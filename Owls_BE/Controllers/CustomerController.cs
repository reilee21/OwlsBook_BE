using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.Repositories.OrderRepos;
using Owls_BE.Repositories.ReviewRepos;
using Owls_BE.Repositories.UserRepos.CustomerRepos;
using System.Security.Claims;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepos repos;
        private readonly IOrderRepos orderRepos;
        private readonly IReviewRepos reviewRepos;


        public CustomerController(ICustomerRepos repos, IOrderRepos orderRepos, IReviewRepos reviewRepos)
        {
            this.repos = repos;
            this.orderRepos = orderRepos;
            this.reviewRepos = reviewRepos;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            string usname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            if (usname == null)
            { return BadRequest(); }
            var profile = await repos.GetProfile(usname);
            if (profile == null)
            { return NotFound(); }
            return Ok(profile);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(CustomerProfileUpdate customer)
        {
            string usname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            if (usname == null)
            { return BadRequest(); }

            var rs = await repos.UpdateProfile(customer, usname);
            return Ok(rs);
        }
        [HttpPost]

        public async Task<IActionResult> UpdatePassword(CustomerUpdateLogin customer)
        {
            string usname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            if (usname == null)
            { return BadRequest(); }

            var rs = await repos.UpdateLogin(customer, usname);

            return rs ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int page = 1, int pageSize = 5)
        {
            string usname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            if (usname == null)
            { return BadRequest(); }
            var rs = await orderRepos.GetOrdersByUser(usname, page, pageSize);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var rs = await orderRepos.GetOrderDetails(orderId);
            if (rs == null)
            { return NotFound(); }
            return Ok(rs);
        }
        [HttpPost]
        public async Task<IActionResult> UploadReview(UserReviewBook review)
        {
            string usname = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            if (usname == null)
            { return BadRequest(); }
            await reviewRepos.UploadReview(review, usname);
            return Ok(StatusCode(200));
        }
        [HttpPost]
        public async Task<IActionResult> CancelOrder(OrderVM order)
        {
            string orderId = order.OrderId;
            if (string.IsNullOrEmpty(orderId))
            { return BadRequest(); }
            var rs = await orderRepos.CancelOrder(orderId);
            if (rs == null)
                return NotFound();
            if (rs.IsError)
                return BadRequest(rs);
            return Ok(rs);
        }
    }
}
