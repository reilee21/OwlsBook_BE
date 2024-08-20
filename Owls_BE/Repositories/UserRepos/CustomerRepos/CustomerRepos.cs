using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Repositories.UserRepos.CustomerRepos
{
    public class CustomerRepos : ICustomerRepos
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;


        public CustomerRepos(Owls_BookContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CustomerRead> GetProfile(string username)
        {
            var c = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (c == null) return null;
            CustomerRead customer = mapper.Map<CustomerRead>(c);

            customer.Ward = LocationService.GetWardId(customer.City, customer.District, customer.Ward);
            customer.District = LocationService.GetDistrictId(customer.City, customer.District);
            customer.City = LocationService.GetCityId(customer.City);

            return customer;
        }

        public async Task<bool> UpdateLogin(CustomerUpdateLogin customer, string username)
        {
            Customer cus = await context.Customers.FirstOrDefaultAsync(p => p.Username.Equals(username));

            cus.Password = customer.NewPassword.Trim();
            try
            {
                context.Entry(cus).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<CustomerProfileUpdate> UpdateProfile(CustomerProfileUpdate customer, string username)
        {
            Customer cus = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (cus == null) return null;
            cus.Ward = LocationService.GetWardName(customer.City, customer.District, customer.Ward);
            cus.District = LocationService.GetDistrictName(customer.City, customer.District);
            cus.City = LocationService.GetCityName(customer.City);

            cus.Address = customer.Address.Trim();
            cus.CustomerName = customer.LastName.Trim() + " " + customer.FirstName.Trim();
            cus.PhoneNumber = customer.PhoneNumber.Trim();
            context.Entry(cus).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return customer;
        }


    }
}
