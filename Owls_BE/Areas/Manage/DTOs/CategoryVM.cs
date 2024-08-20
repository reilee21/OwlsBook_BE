using System.ComponentModel.DataAnnotations;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class CategoryCreate
    {
        [MinLength(4)]
        public string? CategoryName { get; set; }
        public string? ParentCategoryName { get; set; }
    }
    public class CategoryEdit : CategoryCreate
    {
        public int CategoryId { get; set; }

    }
}
