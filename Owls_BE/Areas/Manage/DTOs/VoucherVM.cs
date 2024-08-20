using Owls_BE.Helper;
using System.ComponentModel.DataAnnotations;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class VoucherCreate
    {
        [MinLength(4)]
        public string Code { get; set; }
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; } = 0;
        public VoucherType Type { get; set; }
        [Range(0, double.MaxValue)]
        public double? Value { get; set; }
        [Range(0, double.MaxValue)]
        public double? MinOrderValue { get; set; } = 0;
    }
    public class VoucherEdit : VoucherCreate
    {
        public int VoucherId { get; set; }
        public bool? Active { get; set; } = false;
    }
}
