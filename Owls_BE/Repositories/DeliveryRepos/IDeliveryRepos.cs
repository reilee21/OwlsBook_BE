using Owls_BE.DTOs.Request;
using Owls_BE.Models;

namespace Owls_BE.Repositories.DeliveryRepos
{
    public interface IDeliveryRepos
    {
        Task<Delivery> GetDelivery(DeliveryVM delivery);
    }
}
