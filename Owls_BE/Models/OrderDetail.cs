using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class OrderDetail
    {
        public int? Quantity { get; set; }
        public double? SalePrice { get; set; }
        public double? DiscountPercent { get; set; }
        public string OrderId { get; set; } = null!;
        public int BookId { get; set; }
        public bool? IsRated { get; set; }

        public virtual Book Book { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
