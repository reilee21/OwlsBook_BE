using Owls_BE.DTOs.Response;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class AccountProfile : CustomerRead
    {
        public string Role { get; set; }
        public string AccountId { get; set; }
    }

    public class GrantRole
    {
        public string Role { get; set; }
        public string Username { get; set; }
    }

}
