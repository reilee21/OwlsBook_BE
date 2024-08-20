using AutoMapper;
using Crawler;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<BookCreate, Book>().ReverseMap();
            CreateMap<BookEdit, Book>();

            CreateMap<BookCrawlDTO, BookCrawl>();
        }

    }
}
