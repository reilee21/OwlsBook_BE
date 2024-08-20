namespace Owls_BE.DTOs.Response
{
    public class ReviewBookDetail
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; }
        public string Comment { get; set; }
        public int RatingPoint { get; set; }
    }
}
