namespace Owls_BE.DTOs.Response
{
    public class BookReadVM
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public int? Quantity { get; set; }
        public double? SalePrice { get; set; }
        public double? SalePriceAfterDiscount { get; set; }
        public string? Image { get; set; }
        public int TotalDiscount { get; set; }
        public double? AvgRatingPoint { get; set; }
    }
    public class BookDetailVM
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Format { get; set; }
        public string? Author { get; set; }
        public int? PublishedYear { get; set; }
        public string? Publisher { get; set; }
        public int? PageCount { get; set; }
        public string? Summary { get; set; }
        public int? Quantity { get; set; }
        public double? SalePrice { get; set; }
        public int TotalDiscount { get; set; }
        public double? SalePriceAfterDiscount { get; set; }


        public IEnumerable<string> Image { get; set; }
        public IEnumerable<ReviewBookDetail> Reviews { get; set; }
    }
}
