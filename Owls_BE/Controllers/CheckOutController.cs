using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Repositories.OrderRepos;
using Owls_BE.Services.PaymentSV;
using System.Security.Claims;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CheckOutController : ControllerBase
    {
        private readonly IOrderRepos orderRepos;
        private readonly IPayment payment;
        public CheckOutController(IOrderRepos order, IPayment payment)
        {
            this.orderRepos = order;
            this.payment = payment;
        }

        [HttpPost]
        public async Task<IActionResult> Process(CheckOutVM checkOut)
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var order = await orderRepos.CreateOrder(checkOut, username);
            CheckOutResult result = new CheckOutResult() { Data = order };
            if (order.PaymentMethod == PaymentMethod.BANK.ToString())
            {
                var paymentlink = await payment.CreatePaymentLink(order, checkOut);
                result.Payment = paymentlink;
            }

            return Ok(result);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> HandlePaymentCallback(PaymentCallback callback)
        {
            int transId;
            try
            {
                transId = int.Parse(callback.TransId);
                await orderRepos.HandleCallBack(transId);
                return Ok(callback);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed handle callback." + ex.Message);
            }
        }

    }
}
