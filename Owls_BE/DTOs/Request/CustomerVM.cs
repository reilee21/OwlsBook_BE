namespace Owls_BE.DTOs.Request
{
    public class CusProFileAdminUpdate
    {
        public int CustomerId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Active { get; set; }
        public string? Email { get; set; }
    }
    public class CustomerProfileUpdate
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class CustomerUpdateLogin
    {
        public string? NewPassword { get; set; }
    }

}
