using Owls_BE.Helper;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class FilterRequest
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 24;
    }
    public class ProductFilter : FilterRequest
    {
        public string? SearchName { get; set; } = string.Empty;
        public string? Category { get; set; }
    }

    public class CategoryFilter : FilterRequest
    {
        public string? SearchName { get; set; }
    }
    public class DeliveryFilter : CategoryFilter { }
    public class ReviewFilter : CategoryFilter { }
    public class OrderFilter : FilterRequest
    {
        public string? Search { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public OrderStatus? Status { get; set; }
    }

}
