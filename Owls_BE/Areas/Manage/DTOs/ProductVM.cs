using System.ComponentModel.DataAnnotations;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class BookCreate
    {
        public string? BookTitle { get; set; }
        public string? Format { get; set; }
        public string? Author { get; set; }
        public int? PublishedYear { get; set; }
        public string? Publisher { get; set; }
        [Range(0, int.MaxValue)]
        public int? PageCount { get; set; }
        public string? Summary { get; set; }
        public int? Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public double? SalePrice { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? Images { get; set; }
        public List<int>? DiscountsList { get; set; }
    }
    public class BookEdit : BookCreate
    {
        public int BookId { get; set; }
        public bool? IsActive { get; set; } = false;
    }

}
