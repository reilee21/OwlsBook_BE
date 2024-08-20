using AutoMapper;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartRead>().ReverseMap();
            CreateMap<Cart, CartVM>().ReverseMap();

            CreateMap<CartVM, CartRead>().ReverseMap();

        }
    }
}
