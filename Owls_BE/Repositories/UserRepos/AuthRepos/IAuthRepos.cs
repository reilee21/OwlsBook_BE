using NuGet.Common;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Repositories.UserRepos.AuthRepos
{
	public interface IAuthRepos
	{
		Task Logout(string accountId);
		Task<JWTToken> RefreshToken(JWTToken oldToken);
        Task<JWTToken> Login(LoginModel login);


    }

}
