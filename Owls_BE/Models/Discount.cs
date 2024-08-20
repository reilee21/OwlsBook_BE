using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Books = new HashSet<Book>();
        }

        public int DiscountId { get; set; }
        public double? DiscountPercent { get; set; }
        public bool? Active { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DiscountName { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
