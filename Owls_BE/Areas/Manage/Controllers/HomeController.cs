using Microsoft.AspNetCore.Mvc;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index() => Ok();
    }
}
