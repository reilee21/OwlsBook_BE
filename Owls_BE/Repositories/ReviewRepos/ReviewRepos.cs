using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.Models;

namespace Owls_BE.Repositories.ReviewRepos
{
    public class ReviewRepos : IReviewRepos
    {
        private readonly Owls_BookContext context;

        public ReviewRepos(Owls_BookContext context)
        {
            this.context = context;
        }

        public async Task UploadReview(UserReviewBook review, string username)
        {
            var user = await context.Customers.FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (user == null)
            { throw new Exception("User Not Found"); }

            List<Review> userReviews = new List<Review>();

            foreach (var rv in review.BookReviews)
            {
                userReviews.Add(new Review
                {
                    Comment = rv.Comment,
                    RatingPoint = rv.RatingPoint,
                    CustomerId = user.CustomerId,
                    CreateAt = DateTime.Now,
                    BookId = rv.BookId,
                    Status = true,
                });
            };
            try
            {
                context.Reviews.AddRange(userReviews);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Faild to upload review");
            }
            context.OrderDetails
                .Where(o => o.OrderId == review.OrderId)
                .ToList()
                .ForEach(o => o.IsRated = true);
            await context.SaveChangesAsync();
        }
    }
}
