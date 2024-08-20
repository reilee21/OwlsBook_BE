namespace Owls_BE.DTOs.Response
{
    public class VoucherRead
    {
        public int VoucherId { get; set; }
        public string? Code { get; set; }
        public bool? Active { get; set; }
        public int? Quantity { get; set; }
        public string? Type { get; set; }
        public double? Value { get; set; }
        public double? MinOrderValue { get; set; }
    }
}
