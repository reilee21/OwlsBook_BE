using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int? CustomerId { get; set; }
        public string? Comment { get; set; }
        public int? RatingPoint { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? BookId { get; set; }
        public bool? Status { get; set; }

        public virtual Book? Book { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
