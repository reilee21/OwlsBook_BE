using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.Repositories.CartRepos;
using System.Security.Claims;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartRepos cartRepos;

        public CartController(ICartRepos cartRepos)
        {
            this.cartRepos = cartRepos;
        }

        [Authorize(Policy = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await cartRepos.GetAllCart());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();
            var rs = await cartRepos.GetCartByUser(username);
            return Ok(rs);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(CartVM item)
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();
            var rs = await cartRepos.AddToCart(item, username);
            return Ok(rs);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateCart(CartVM cart)
        {
            var rs = await cartRepos.UpdateCart(cart);
            return Ok(rs);
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveItem(CartVM cart)
        {
            var rs = await cartRepos.DeleteCart(cart);
            return Ok(rs);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (username == null) return Unauthorized();
            var rs = await cartRepos.Checkout(username);
            return Ok(rs);
        }
    }
}
