using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Repositories.CartRepos
{
    public class CartRepos : ICartRepos
    {
        private readonly Owls_BookContext context;
        private readonly IMapper mapper;
        private readonly DapperContext dapper;


        public CartRepos(Owls_BookContext context, IMapper mapper, DapperContext dapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.dapper = dapper;
        }

        public async Task<bool> AddToCart(CartVM cart, string userName)
        {
            var u = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(userName));
            if (u == null)
            { return false; }
            int newQuan = cart.Quantity;
            Cart ct = await context.Carts
                        .FirstOrDefaultAsync(ct => ct.CustomerId.Equals(u.CustomerId)
                                                && ct.BookId.Equals(cart.BookId));

            if (ct != null)
            {
                newQuan += (int)ct.Quantity;
            }

            bool check = await CheckQuantity(newQuan, (int)cart.BookId);
            if (!check)
            { return false; }

            Cart newCart;

            if (ct == null)
            {
                newCart = new Cart()
                { BookId = cart.BookId, CustomerId = u.CustomerId, Quantity = newQuan };
                context.Carts.Add(newCart);
            }
            else
            {
                newCart = new Cart()
                { Id = ct.Id, CustomerId = ct.CustomerId, BookId = ct.BookId, Quantity = newQuan };
                context.Entry(ct).CurrentValues.SetValues(newCart);
            }
            await context.SaveChangesAsync();
            return true;

        }

        public async Task<Cart> DeleteCart(CartVM cart)
        {
            Cart c = await context.Carts.FindAsync(cart.Id);
            if (c == null)
                return null;
            context.Carts.Remove(c);
            await context.SaveChangesAsync();
            return c;
        }

        public async Task<IEnumerable<CartRead>> GetAllCart()
        {
            var c = await context.Carts.ToListAsync();
            var rs = mapper.Map<IEnumerable<CartRead>>(c);
            return rs;
        }

        public async Task<IEnumerable<CartRead>> GetCartByUser(string userName)
        {
            var u = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(userName));
            if (u == null)
                return null;

            using (var connection = dapper.CreateConnection())
            {
                var qr = $"exec GetCustomerCart {u.CustomerId}";
                var rs = await connection.QueryAsync<CartRead>(qr);
                rs = rs.Where(i => i.BookId != 0);

                return rs;
            }


        }

        public async Task<bool> UpdateCart(CartVM cart)
        {
            Cart c = await context.Carts.FindAsync(cart.Id);
            if (c == null)
                return false;
            bool check = await CheckQuantity(cart.Quantity, (int)c.BookId);
            if (!check)
                return false;
            c.Quantity = cart.Quantity;
            context.Entry(c).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> CheckQuantity(int quantity, int bookId)
        {
            var b = await context.Books.FindAsync(bookId);
            if (b == null)
            { return false; }
            return quantity <= b.Quantity;
        }

        public async Task<IEnumerable<CartRead>> Checkout(string username)
        {
            var u = await context.Customers.FirstOrDefaultAsync(c => c.Username.Equals(username));
            if (u == null)
                return null;

            using (var connection = dapper.CreateConnection())
            {
                var qr = $"exec GetCustomerCart {u.CustomerId}";
                var carts = await connection.QueryAsync<CartRead>(qr);
                var rs = carts.Where(c => c.isActive == true && !string.IsNullOrEmpty(c.BookTitle));
                return rs;
            }
        }
    }
}
