using Net.payOS.Types;
using Owls_BE.DTOs.Request;
using Owls_BE.Models;
using Payment;

namespace Owls_BE.Services.PaymentSV
{
    public class PaymentSV : IPayment
    {
        private readonly IConfiguration configuration;
        private readonly IPaymentServices payment;
        public PaymentSV(IConfiguration configuration, IPaymentServices paymentServices)
        {
            this.configuration = configuration;
            this.payment = paymentServices;
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(Order order, CheckOutVM checkOut)
        {
            string[] orderId = order.OrderId.Split('-');
            int transId = int.Parse(orderId[5]);
            List<ItemData> items = new List<ItemData>();
            foreach (var item in checkOut.Carts)
            {
                items.Add(new ItemData(name: item.BookTitle, price: (int)item.SalePriceAfterDiscount, quantity: item.Quantity));
            }

            PaymentData paymentData = new PaymentData
            (
                // amount: (int)order.Total,
                amount: 5000,
                orderCode: transId,
                items: items,
                description: $"Thanh toan don hang Owls",
                returnUrl: configuration["PaymentConfig:SUCCESS_URL_CALLBACK"],
                cancelUrl: configuration["PaymentConfig:CANCEL_URL_CALLBACK"],
                expiredAt: (int)DateTimeOffset.Now.AddMinutes(20).ToUnixTimeSeconds()
            );
            try
            {
                var paymentlink = await payment.CreatePaymentLink(paymentData);
                return paymentlink;
            }
            catch (Exception ex)
            {
                throw new Exception("------------------------\n Failed to create payment link" + ex.Message);
            }
        }
        public async Task<PaymentLinkInformation> GetPaymentInfomation(int transactionId)
        {
            try
            {
                var paymentinfo = await payment.GetPaymentInfomation(transactionId);
                return paymentinfo;
            }
            catch (Exception ex)
            {
                throw new Exception("------------------------\n Failed to get payment information" + ex.Message);
            }
        }
        public async Task<PaymentLinkInformation> CancelPayment(int transactionId)
        {
            throw new NotImplementedException();

        }


    }
}
