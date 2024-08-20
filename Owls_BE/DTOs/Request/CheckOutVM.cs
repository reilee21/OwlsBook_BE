using Owls_BE.DTOs.Response;

namespace Owls_BE.DTOs.Request
{
    public class CheckOutVM
    {
        public List<CartRead> Carts { get; set; } = new();
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Voucher { get; set; }
        public int PaymentMethod { get; set; } = 0;
    }
    public class PaymentCallback
    {
        public string TransId { get; set; }
    }
}
