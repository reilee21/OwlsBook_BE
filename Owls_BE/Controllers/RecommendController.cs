using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Models;
using Owls_BE.Repositories.BookRepos;
using Recommender;
using System.Security.Claims;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RecommendController : ControllerBase
    {
        private readonly IRecommenderByUser _recommender;

        private readonly IBookRepos bookRepos;
        private readonly Owls_BookContext context;

        public RecommendController(IRecommenderByUser recommender, IBookRepos repos, Owls_BookContext bcontext)
        {
            _recommender = recommender;
            bookRepos = repos;
            context = bcontext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookRecommend(int? quantity)
        {
            if (!quantity.HasValue)
                quantity = 12;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (username == null)
            {
                var newBooks = await bookRepos.GetNewBooks((int)quantity, null);
                return Ok(newBooks);
            }

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (customer == null)
                return Unauthorized();

            try
            {
                var predictions = _recommender.Predict(customer.CustomerId);

                List<int> bookIds = predictions.Select(prediction => (int)prediction.ProductId).Take((int)quantity).ToList();

                var result = (await bookRepos.GetByListId(bookIds)).ToList();

                if (result.Count < (int)quantity)
                {
                    var additionalBooksCount = (int)quantity - result.Count;
                    var newBooks = await bookRepos.GetNewBooks(additionalBooksCount, bookIds);
                    result.AddRange(newBooks);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




    }
}
