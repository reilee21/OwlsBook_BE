using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? Active { get; set; }
        public string? Email { get; set; }
        public string? LoginId { get; set; }

        public virtual UserLogin? Login { get; set; }
    }
}
