using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Cart
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public int? BookId { get; set; }
        public int? CustomerId { get; set; }

        public virtual Book? Book { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
