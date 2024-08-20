namespace Owls_BE.DTOs.Response
{
    public class OrderBaseResponse
    {

        public string OrderId { get; set; }
        public double? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public bool? IsPaid { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? TransactionId { get; set; }
        public string? Name { get; set; }
        public string? Phonenumber { get; set; }
        public double? ShippingFee { get; set; }
        public double? VoucherDiscountAmount { get; set; }

        public List<OrderDetailResponse> OrderDetails { get; set; }
    }
    public class OrderDetailResponse
    {
        public int Quantity { get; set; }
        public double SalePrice { get; set; }
        public double DiscountPercent { get; set; }
        public bool IsRated { get; set; }
        public OrderBookResponse Book { get; set; } = null!;
    }
    public class OrderBookResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

    }


    public class UpdateResponse<T>
    {
        public bool IsError { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Value { get; set; }
    }
}
