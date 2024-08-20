using Owls_BE.DTOs.Request;
using Owls_BE.Models;

namespace Owls_BE.Repositories.VoucherRepos
{
    public interface IVoucherRepos
    {
        Task<IEnumerable<Voucher>> GetVouchersAsync();
        Task<Voucher> GetVoucher(string vouchercode);
        Task<Voucher> AddVoucher(Voucher voucher);
        Task<Voucher> DeleteVoucher(string voucherId);
        Task<Voucher> UpdateVoucher(Voucher voucher);

        Task<string> CheckApplyVoucher(VoucherApply voucherApply, string username);

    }
}
