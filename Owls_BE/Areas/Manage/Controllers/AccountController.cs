using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;
        public AccountController(Owls_BookContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetStaff()
        {
            var rs = context.Admins.ToList();
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetAccountProfile(string? username)
        {
            if (string.IsNullOrEmpty(username))
            { return Ok(); }
            Customer customer = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username.Trim()));
            if (customer == null)
                return NotFound();
            AccountProfile account = mapper.Map<AccountProfile>(customer);
            account.Role = UserRole.Customer;

            Admin a = await context.Admins.FirstOrDefaultAsync(c => c.Username.Equals(username.Trim()));
            if (a != null)
            {
                account.Role = a.Role;
            }
            var acc = await context.UserLogins.FirstOrDefaultAsync(u => u.Username.Equals(username.Trim()));
            if (acc != null)
            {
                account.AccountId = acc.Id;
            }
            return Ok(account);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole(GrantRole model)
        {
            Customer customer = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(model.Username.Trim()));

            if (customer == null)
                return NotFound();
            Admin admin = await context.Admins.FirstOrDefaultAsync(a => a.Username.Equals(model.Username));
            if (model.Role != UserRole.Customer)
            {
                if (admin != null)
                {
                    admin.Role = model.Role;
                    context.Entry(admin).State = EntityState.Modified;
                }
                else
                {
                    admin = new Admin
                    {
                        Active = true,
                        Username = model.Username,
                        PhoneNumber = customer.PhoneNumber,
                        Email = customer.Email,
                        Role = model.Role,
                        LoginId = string.IsNullOrEmpty(customer.LoginId) ? customer.LoginId : "",
                    };
                    context.Admins.Add(admin);
                }
            }
            else
            {
                context.Admins.Remove(admin);
            }
            await context.SaveChangesAsync();

            return Ok(admin);

        }


    }
}
