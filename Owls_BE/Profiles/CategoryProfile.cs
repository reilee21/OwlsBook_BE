using AutoMapper;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryRead>().ReverseMap();
            CreateMap<CategoryCreate, Category>();
            CreateMap<CategoryEdit, Category>();

        }
    }
}
