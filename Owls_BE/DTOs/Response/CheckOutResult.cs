using Net.payOS.Types;
using Owls_BE.Models;

namespace Owls_BE.DTOs.Response
{
    public class CheckOutResult
    {
        public Order Data { get; set; }
        public CreatePaymentResult? Payment { get; set; }

    }
}
