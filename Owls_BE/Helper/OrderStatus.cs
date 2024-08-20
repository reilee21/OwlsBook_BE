namespace Owls_BE.Helper
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Delivered,
        Cancelled,
        CustomerCancelled
    }

    public enum PayOSStatus
    {
        PENDING,
        PAID,
        CANCELLED,
        PROCESSING,
        EXPIRED
    }
}
