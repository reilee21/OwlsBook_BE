using Owls_BE.DTOs.Request;
using Owls_BE.DTOs.Response;
using Owls_BE.Models;

namespace Owls_BE.Repositories.OrderRepos
{
    public interface IOrderRepos
    {
        Task<Order> CreateOrder(CheckOutVM model, string username);
        Task<PageList<OrderBaseResponse>> GetOrdersByUser(string username, int page, int pageSize);
        Task<OrderBaseResponse> GetOrderDetails(string orderId);
        Task<UpdateResponse<OrderBaseResponse>> CancelOrder(string orderId);

        Task HandleCallBack(int transId);

    }
}
