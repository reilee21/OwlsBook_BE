using Dapper;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Repositories.VoucherRepos
{
    public class VoucherRepos : IVoucherRepos
    {
        private readonly Owls_BookContext _bookContext;
        private readonly DapperContext dapper;


        public VoucherRepos(Owls_BookContext bookContext, DapperContext dapper)
        {
            _bookContext = bookContext;
            this.dapper = dapper;
        }

        public async Task<Voucher> AddVoucher(Voucher voucher)
        {
            voucher.Code = voucher.Code.Trim().ToUpper();
            await _bookContext.AddAsync(voucher);
            await _bookContext.SaveChangesAsync();
            return voucher;
        }

        public async Task<string> CheckApplyVoucher(VoucherApply voucherApply, string username)
        {
            var voucher = await _bookContext.Vouchers.FirstOrDefaultAsync(v => v.Code.Equals(voucherApply.Code.Trim().ToUpper()));
            if (voucher == null)
            { return ""; }
            var u = await _bookContext.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (voucher.Quantity < 1)
                return "quantity";
            using (var connection = dapper.CreateConnection())
            {
                var qr = $"exec GetCustomerCart {u.CustomerId}";
                var rs = await connection.QueryAsync<CartRead>(qr);
                var amount = rs.Where(c => c.isActive == true || !string.IsNullOrEmpty(c.BookTitle)).Sum(p => (p.Quantity * p.SalePriceAfterDiscount));
                if (amount < voucher.MinOrderValue)
                    return "amount";
            }
            return "Ok";
        }

        public async Task<Voucher> DeleteVoucher(string voucherId)
        {
            Voucher d = await _bookContext.Vouchers.FindAsync(int.Parse(voucherId));
            if (d == null)
                return null;

            _bookContext.Vouchers.Remove(d);
            await _bookContext.SaveChangesAsync();
            return d;
        }

        public async Task<Voucher> GetVoucher(string vouchercode)
        {
            var rs = await _bookContext.Vouchers.FirstOrDefaultAsync(v => v.Code.Equals(vouchercode.Trim().Normalize()));
            return rs;
        }

        public async Task<IEnumerable<Voucher>> GetVouchersAsync()
        {
            var rs = await _bookContext.Vouchers.ToListAsync();
            return rs;
        }

        public async Task<Voucher> UpdateVoucher(Voucher voucher)
        {
            var existingVoucher = await _bookContext.Vouchers.FindAsync(voucher.VoucherId);
            if (existingVoucher == null)
            {
                return null;
            }
            voucher.Code = voucher.Code.Trim().ToUpper();
            _bookContext.Entry(existingVoucher).CurrentValues.SetValues(voucher);

            await _bookContext.SaveChangesAsync();
            return existingVoucher;
        }
    }
}
