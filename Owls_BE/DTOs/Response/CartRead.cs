namespace Owls_BE.DTOs.Response
{
    public class CartRead
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public double? SalePrice { get; set; }
        public double? SalePriceAfterDiscount { get; set; }
        public string? Image { get; set; }
        public int TotalDiscount { get; set; }
        public bool isActive { get; set; } = false;
    }
}
