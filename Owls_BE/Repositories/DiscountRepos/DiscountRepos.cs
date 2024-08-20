using Microsoft.EntityFrameworkCore;
using Owls_BE.Models;

namespace Owls_BE.Repositories.DiscountRepos
{
    public class DiscountRepos : IDiscountRepos
    {
        private readonly Owls_BookContext _bookContext;

        public DiscountRepos(Owls_BookContext bookContext)
        {
            _bookContext = bookContext;
        }


        public async Task<Discount> GetDiscountById(string discountId)
        {
            var rs = await _bookContext.Discounts.FindAsync(int.Parse(discountId));
            return rs;
        }

        public async Task<IEnumerable<Discount>> GetDiscountsAsync()
        {
            var rs = await _bookContext.Discounts.ToListAsync();
            return rs;
        }



    }
}
