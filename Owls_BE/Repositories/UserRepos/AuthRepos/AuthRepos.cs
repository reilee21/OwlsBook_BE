using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;
using System.Security.Claims;

namespace Owls_BE.Repositories.UserRepos.AuthRepos
{
    public class AuthRepos : IAuthRepos
    {
        private readonly Owls_BookContext owlsContext;
        private readonly IConfiguration configuration;

        public AuthRepos(Owls_BookContext owlsContext, IConfiguration configuration)
        {
            this.owlsContext = owlsContext;
            this.configuration = configuration;
        }

        public async Task Logout(string accountId)
        {
            var uslogin = await owlsContext.UserLogins.FindAsync(accountId);

            if (uslogin == null)
                return;
            uslogin.Refreshtoken = "";
            await owlsContext.SaveChangesAsync();
        }

        public async Task<JWTToken> RefreshToken(JWTToken oldToken)
        {
            JWTToken newToken = new JWTToken();

            var principal = JWTHelper.GetClaimsPrincipal(oldToken.AccessToken, configuration["TokenOptions:AccessTokenSecurityKey"]);
            if (principal == null)
            { return newToken; }

            var username = principal.Identity.Name;
            var role = principal.Claims.First(c => c.Type == ClaimTypes.Role).Value;


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                return newToken;
            }

            var uslogin = await owlsContext.UserLogins.FirstOrDefaultAsync(u => u.Username.Equals(username) && u.Refreshtoken.Equals(oldToken.RefreshToken));
            if (uslogin == null)
            {
                return newToken;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.Role,role),
            };

            newToken.AccessToken = JWTHelper.GenerateJWTToken(claims, configuration["TokenOptions:AccessTokenSecurityKey"], 1);
            newToken.RefreshToken = Guid.NewGuid().ToString();

            uslogin.Refreshtoken = newToken.RefreshToken;
            await owlsContext.SaveChangesAsync();

            return newToken;
        }

        private async Task<bool> AccountExist(string username)
        {
            var a = await owlsContext.Admins.FirstOrDefaultAsync(a => a.Username.Equals(username));
            if (a != null)
                return true;
            var b = await owlsContext.Customers.FirstOrDefaultAsync(b => b.Username.Equals(username));
            if (b != null)
                return true;
            return false;

        }
        private async Task<bool> AccountActive(int id)
        {
            var c = await owlsContext.Customers.FindAsync(id);
            if (c != null)
            {
                return c.Active == true ? true : false;
            }
            var a = await owlsContext.Admins.FindAsync(id);
            return a.Active == true ? true : false;
        }

        private Customer Register(LoginModel acc)
        {
            Customer customer = new Customer() { Active = true, Username = acc.Username.Trim(), Email = acc.Email.Trim(), LoginId = acc.ClerkID };
            return customer;
        }
        public async Task<JWTToken> Login(LoginModel login)
        {
            bool accountReistered = await AccountExist(login.Username);

            UserLogin newLogin = new UserLogin() { Id = login.ClerkID, Username = login.Username, Refreshtoken = Guid.NewGuid().ToString() };

            if (!accountReistered) // chưa đk tài khoản -- khách
            {
                Customer newCustomer = Register(login);
                newLogin.Role = UserRole.Customer;
                owlsContext.UserLogins.Add(newLogin);
                owlsContext.Customers.Add(newCustomer);
                await owlsContext.SaveChangesAsync();

            }
            else // Đã có tk trong hệ thống
            {
                Admin a = await owlsContext.Admins.FirstOrDefaultAsync(a => a.Username.Equals(login.Username));
                if (a != null) // là nhân viên
                {
                    bool active = await AccountActive(a.AdminId);
                    if (!active)
                        return new JWTToken() { AccessToken = "block" };
                    newLogin.Role = a.Role;
                    a.LoginId = login.ClerkID;
                    owlsContext.Entry(a).State = EntityState.Modified;
                }
                else // là khách
                {
                    Customer c = await owlsContext.Customers.FirstOrDefaultAsync(c => c.Username.Equals(login.Username));
                    bool active = await AccountActive(c.CustomerId);
                    if (!active)
                        return new JWTToken() { AccessToken = "block" };
                    newLogin.Role = UserRole.Customer;
                    c.LoginId = login.ClerkID;
                    owlsContext.Entry(c).State = EntityState.Modified;

                }
            }

            var claims = new[]
               {
                    new Claim(ClaimTypes.Name,newLogin.Username),
                    new Claim(ClaimTypes.Role,newLogin.Role),
                };
            JWTToken token = new JWTToken()
            {
                AccessToken = JWTHelper.GenerateJWTToken(claims, configuration["TokenOptions:AccessTokenSecurityKey"], 1),
                RefreshToken = newLogin.Refreshtoken
            };

            var lg = await owlsContext.UserLogins.FindAsync(newLogin.Id);
            if (lg != null)
                owlsContext.Entry(lg).CurrentValues.SetValues(newLogin);
            else
                owlsContext.UserLogins.Add(newLogin);

            await owlsContext.SaveChangesAsync();

            return token;
        }


    }
}
