using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;

namespace Owls_BE.Repositories.UserRepos.CustomerRepos
{
    public interface ICustomerRepos
    {
        Task<CustomerRead> GetProfile(string username);
        Task<CustomerProfileUpdate> UpdateProfile(CustomerProfileUpdate customer,string username);
        Task<bool> UpdateLogin(CustomerUpdateLogin customer,string username);


    }
}
