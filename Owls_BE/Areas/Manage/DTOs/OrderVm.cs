using Net.payOS.Types;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class OrderManageResponse
    {
        public string OrderId { get; set; } = null!;
        public double? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public bool? IsPaid { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? TransactionId { get; set; }
        public Voucher? Voucher { get; set; }
        public string? Name { get; set; }
        public string? Phonenumber { get; set; }
        public double? ShippingFee { get; set; }
        public PaymentLinkInformation? Payment { get; set; }
        public List<OrderManageDetailResponse> Details { get; set; } = new();
    }
    public class OrderManageDetailResponse
    {
        public int? Quantity { get; set; }
        public double? SalePrice { get; set; }
        public double? DiscountPercent { get; set; }
        public string OrderId { get; set; } = null!;
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? ImageThumbnail { get; set; }

    }

    public class OrderManageUpdate
    {
        public string OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public bool? IsPaid { get; set; } = false;

    }
}
