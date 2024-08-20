namespace Owls_BE.DTOs.Response
{
    public class CustomerRead
    {
        public string? Username { get; set; }
        public string? CustomerName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Active { get; set; }
        public string? Email { get; set; }
    }

}
