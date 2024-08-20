using Microsoft.EntityFrameworkCore;
using Owls_BE.DTOs.Request;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Repositories.DeliveryRepos
{
    public class DeliveryRepos : IDeliveryRepos
    {
        private readonly Owls_BookContext _bookContext;

        public DeliveryRepos(Owls_BookContext bookContext)
        {
            _bookContext = bookContext;
        }

        public async Task<Delivery> GetDelivery(DeliveryVM delivery)
        {
            string city = LocationService.GetCityName(delivery.City);
            string district = LocationService.GetDistrictName(delivery.City, delivery.District);
            IEnumerable<Delivery> list = await _bookContext.Deliveries.Where(d => d.City.Equals(city)).ToListAsync();
            Delivery rs = null;
            if (!string.IsNullOrEmpty(district))
            {
                rs = list.FirstOrDefault(d => d.District != null && d.District.Equals(district));
            }
            if (rs == null || string.IsNullOrEmpty(district))
            {
                rs = list.First();

            }

            return rs;
        }
    }
}
