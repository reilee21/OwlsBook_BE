using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Book
    {
        public Book()
        {
            BookImages = new HashSet<BookImage>();
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
            Reviews = new HashSet<Review>();
            Discounts = new HashSet<Discount>();
        }

        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Format { get; set; }
        public string? Author { get; set; }
        public int? PublishedYear { get; set; }
        public string? Publisher { get; set; }
        public int? PageCount { get; set; }
        public string? Summary { get; set; }
        public int? Quantity { get; set; }
        public double? SalePrice { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public int? BookView { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<BookImage> BookImages { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }
    }
}
