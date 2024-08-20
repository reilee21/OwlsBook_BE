using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class UserLogin
    {
        public UserLogin()
        {
            Admins = new HashSet<Admin>();
            Customers = new HashSet<Customer>();
        }

        public string Id { get; set; } = null!;
        public string? Username { get; set; }
        public string? Refreshtoken { get; set; }
        public string? Role { get; set; }

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
