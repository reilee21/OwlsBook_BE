namespace Owls_BE.DTOs.Request
{
    public class OrderVM
    {
        public string OrderId { get; set; } = null!;
        public double? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? TransactionId { get; set; }
        public int? VoucherId { get; set; }
    }
    public class DeliveryVM
    {
        public string City { get; set; }
        public string? District { get; set; }
    }
}
