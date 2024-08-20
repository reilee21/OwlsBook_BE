namespace Owls_BE.DTOs.Response
{
    public class DiscountRead
    {
        public int DiscountId { get; set; }
        public double? DiscountPercent { get; set; }
        public bool? Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DiscountName { get; set; }

    }
}
