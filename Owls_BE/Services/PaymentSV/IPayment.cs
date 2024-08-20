using Net.payOS.Types;
using Owls_BE.DTOs.Request;
using Owls_BE.Models;

namespace Owls_BE.Services.PaymentSV
{
    public interface IPayment
    {
        Task<CreatePaymentResult> CreatePaymentLink(Order order, CheckOutVM checkOut);
        Task<PaymentLinkInformation> GetPaymentInfomation(int transactionId);
        Task<PaymentLinkInformation> CancelPayment(int transactionId);
    }
}
