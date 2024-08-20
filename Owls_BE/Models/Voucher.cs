using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Orders = new HashSet<Order>();
        }

        public int VoucherId { get; set; }
        public string? Code { get; set; }
        public bool? Active { get; set; }
        public int? Quantity { get; set; }
        public string? Type { get; set; }
        public double? Value { get; set; }
        public double? MinOrderValue { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
