using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.Repositories.BookRepos;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepos bookRepos;

        public BooksController(IBookRepos bookRepos)
        {
            this.bookRepos = bookRepos;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Filter filter)
        {
            var rs = await bookRepos.GetAllBooks(filter);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> SearchByName(string searchString, [FromQuery] Filter filter)
        {
            var rs = await bookRepos.SearchBookByName(searchString, filter);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetBookByCate(string nameCate, [FromQuery] Filter filter)
        {
            var rs = await bookRepos.GetBookByCategory(nameCate, filter);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetBookDiscount(int quantity)
        {
            var rs = await bookRepos.GetBookDiscount(quantity, new Filter());
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetBestSell(int quantity)
        {
            var rs = await bookRepos.GetBestSeller(quantity);
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string? name)
        {

            var rs = await bookRepos.Details(name);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

    }
}
