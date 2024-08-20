using AutoMapper;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<DiscountCreate, Discount>();
            CreateMap<DiscountEdit, Discount>();


        }
    }
}
