using System.ComponentModel.DataAnnotations;

namespace Owls_BE.DTOs.Response
{
    public class CategoryRead
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? ParentCategory { get; set; }
        public string? CategoryTag { get; set; }

    }
}
