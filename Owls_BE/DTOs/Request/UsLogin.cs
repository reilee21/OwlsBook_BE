namespace Owls_BE.DTOs.Request
{

    public class LoginModel
    {
        public string Username { get; set; }
        public string ClerkID { get; set; }
        public string? Email { get; set; }
    }
    public class LogoutModel
    {
        public string Uid { get; set; }
    }
}
