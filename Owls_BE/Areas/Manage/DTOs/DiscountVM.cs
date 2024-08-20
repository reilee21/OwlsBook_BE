using System.ComponentModel.DataAnnotations;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class DiscountCreate
    {
        [Range(0, 100)]
        public double? DiscountPercent { get; set; }
        public string? DiscountName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class DiscountEdit : DiscountCreate
    {
        public int DiscountId { get; set; }
        public bool? Active { get; set; } = false;
    }

    public class DiscountOldBook
    {
        public int DiscountId { get; set; }

    }

}
