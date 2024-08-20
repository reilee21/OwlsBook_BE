using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Repositories.CartRepos
{
    public interface ICartRepos
    {
        Task<IEnumerable<CartRead>> GetAllCart();
        Task<bool> AddToCart(CartVM cart, string userName);
        Task<IEnumerable<CartRead>> GetCartByUser(string username);
        Task<bool> UpdateCart(CartVM cart);
        Task<Cart> DeleteCart(CartVM cart);
        Task<IEnumerable<CartRead>> Checkout(string username);


    }
}
