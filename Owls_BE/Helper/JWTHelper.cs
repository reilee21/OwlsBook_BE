using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Owls_BE.Helper
{
	public class JWTHelper
	{
		public static string GenerateJWTToken(Claim[] additionalClaims, string secretkey, int TimeLife)
		{
			var claims = new List<Claim>();
			claims.AddRange(additionalClaims);
			var tokendes = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims.ToArray()),
				Expires = DateTime.UtcNow.AddHours(TimeLife),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretkey)),
					SecurityAlgorithms.HmacSha512Signature)

			};
			var jwttoken = new JwtSecurityTokenHandler();
			var token = jwttoken.CreateToken(tokendes);
			return jwttoken.WriteToken(token);
		}

        public static ClaimsPrincipal GetClaimsPrincipal(string token, string secretKey)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                if (token == null)
                {
                    return null;
                }
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key
                };
                SecurityToken securityToken;
                ClaimsPrincipal claimsPrincipal = handler.ValidateToken(token, parameters, out securityToken);
                return claimsPrincipal;
            }
            catch
            {
                return null;
            }
        }
    }
}
