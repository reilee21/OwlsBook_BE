namespace Owls_BE.DTOs.Request
{
    public class UserReviewBook
    {
        public List<ReviewBook> BookReviews { get; set; }
        public string OrderId { get; set; }
    }
    public class ReviewBook
    {
        public int BookId { get; set; }
        public string? Comment { get; set; }
        public int? RatingPoint { get; set; }

    }
}
