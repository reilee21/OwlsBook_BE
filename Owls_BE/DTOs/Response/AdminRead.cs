namespace Owls_BE.DTOs.Response
{
    public class AdminRead
    {
        public int AdminId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Active { get; set; }
        public string? Email { get; set; }
        public string? AccountId { get; set; }

        public AdminRole UserRole { get; set; }
    }

    public enum AdminRole
    {
        Manager,
        ProductAdmin,
        OrderAdmin
    }
}
