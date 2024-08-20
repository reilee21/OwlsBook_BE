using Microsoft.AspNetCore.Mvc;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Repositories.UserRepos.AuthRepos;

namespace Owls_BE.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepos auth;

        public AuthController(IAuthRepos auth)
        {
            this.auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            JWTToken token = await auth.Login(user);
            if (token.AccessToken.Equals("lock"))
                return BadRequest("Acccount is locked");
            return Ok(token);
        }
        [HttpPost]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            await auth.Logout(model.Uid);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken(JWTToken token)
        {
            JWTToken newToken = await auth.RefreshToken(token);
            if (!string.IsNullOrEmpty(newToken.AccessToken)) return Ok(newToken);
            return Unauthorized();
        }

    }
}
