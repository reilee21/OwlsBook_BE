using System;
using System.Collections.Generic;

namespace Owls_BE.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderId { get; set; } = null!;
        public double? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public bool? IsPaid { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? TransactionId { get; set; }
        public int? VoucherId { get; set; }
        public int? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Phonenumber { get; set; }
        public double? ShippingFee { get; set; }
        public int? DeliveryId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Delivery? Delivery { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
