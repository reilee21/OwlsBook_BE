using AutoMapper;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Profiles
{
    public class Voucherrofile : Profile
    {
        public Voucherrofile()
        {
            CreateMap<Voucher, VoucherRead>();

            CreateMap<VoucherCreate, Voucher>()
                .ForMember(des => des.Type, opt => opt.MapFrom(s => s.Type.ToString()));
            CreateMap<VoucherEdit, Voucher>()
                .ForMember(des => des.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        }
    }
}
