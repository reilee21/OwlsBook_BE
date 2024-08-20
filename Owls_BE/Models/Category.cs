using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Category
    {
        public Category()
        {
            Books = new HashSet<Book>();
            InverseParentCategoryNavigation = new HashSet<Category>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryTag { get; set; }
        public int? ParentCategory { get; set; }

        public virtual Category? ParentCategoryNavigation { get; set; }
        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<Category> InverseParentCategoryNavigation { get; set; }
    }
}
