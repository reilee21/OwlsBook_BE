using AutoMapper;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CusProFileAdminUpdate>().ReverseMap();
            CreateMap<Customer, CustomerRead>().ReverseMap();
            CreateMap<Customer, AccountProfile>().ReverseMap();

        }
    }
}
