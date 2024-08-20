using AutoMapper;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderBaseResponse>().ForMember(des => des.VoucherDiscountAmount, opt => opt.MapFrom(s => s.Voucher.Value));
            CreateMap<OrderDetail, OrderDetailResponse>();
            CreateMap<Book, OrderBookResponse>()
                .ForMember(des => des.Title, opt => opt.MapFrom(s => s.BookTitle));

            CreateMap<Order, OrderManageResponse>()
                .ForMember(des => des.Details, opt => opt.MapFrom(s => s.OrderDetails));
            CreateMap<OrderDetail, OrderManageDetailResponse>();

        }
    }
}
