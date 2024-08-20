using Owls_BE.Models;

namespace Owls_BE.Repositories.DiscountRepos
{
    public interface IDiscountRepos
    {
        Task<IEnumerable<Discount>> GetDiscountsAsync();
        Task<Discount> GetDiscountById(string discountId);

    }
}
