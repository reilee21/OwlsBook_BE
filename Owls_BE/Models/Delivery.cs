using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Delivery
    {
        public Delivery()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public double? ShippingFee { get; set; }
        public int? EstimatedDeliveryTime { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
