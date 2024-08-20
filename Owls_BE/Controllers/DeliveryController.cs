using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.Repositories.DeliveryRepos;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryRepos deliveryRepos;

        public DeliveryController(IDeliveryRepos delivery)
        {
            deliveryRepos = delivery;
        }
        [HttpPost]
        public async Task<IActionResult> GetDelivery(DeliveryVM delivery)
        {
            var rs = await deliveryRepos.GetDelivery(delivery);
            return Ok(rs);
        }
    }
}
