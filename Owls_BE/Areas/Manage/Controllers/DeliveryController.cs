using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Owls_BE.Areas.Manage.DTOs;
using Owls_BE.DTOs.Response;
using Owls_BE.Helper;
using Owls_BE.Models;

namespace Owls_BE.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("api/Manage/[controller]/[action]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly Owls_BookContext context;

        public DeliveryController(Owls_BookContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DeliveryFilter filter)
        {
            PageList<Delivery> rs = new PageList<Delivery>();
            rs.PageIndex = (int)filter.Page;
            rs.PageSize = (int)filter.PageSize;
            if (!string.IsNullOrEmpty(filter.SearchName))
            {
                var c = context.Deliveries.Where(c => c.City.ToUpper().Contains(filter.SearchName.ToUpper().Trim()));

                List<Delivery> items = c.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
                rs.TotalItems = c.Count();
                rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));
                rs.Items = items;
            }
            else
            {
                var c = await context.Deliveries.ToListAsync();
                List<Delivery> items = c.Skip((int)((filter.Page - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
                rs.TotalItems = c.Count();
                rs.TotalPages = (int)Math.Ceiling((decimal)((double)rs.TotalItems / rs.PageSize));
                rs.Items = items;
            }
            return Ok(rs);
        }
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var d = await context.Deliveries.FindAsync(id);
            if (d == null) return NotFound();
            string city = LocationService.GetCityId(d.City);
            string district = "";
            if (!string.IsNullOrEmpty(d.District))
            {
                district = LocationService.GetDistrictId(d.City, d.District);
            }
            DeliveryResponse response = new DeliveryResponse()
            {
                Id = id,
                CityId = city,
                DistrictId = district,
                EstimatedDeliveryTime = d.EstimatedDeliveryTime,
                ShippingFee = d.ShippingFee,
            };
            return (Ok(response));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeliveryCreate deliveryCreate)
        {
            if (ModelState.IsValid)
            {
                string city = ""; string district = "";
                city = LocationService.GetCityName(deliveryCreate.CityId);
                if (!string.IsNullOrEmpty(deliveryCreate.DistrictId))
                {
                    district = LocationService.GetDistrictName(deliveryCreate.CityId, deliveryCreate.DistrictId);
                }
                if (await IsExist(city, district)) return Ok(new ErrorStatus { Code = "11", Message = "Đã có địa điểm này" });
                Delivery delivery = new Delivery
                {
                    District = district,
                    City = city,
                    EstimatedDeliveryTime = deliveryCreate.EstimatedDeliveryTime,
                    ShippingFee = deliveryCreate.ShippingFee,
                };
                try
                {
                    context.Deliveries.Add(delivery);
                    await context.SaveChangesAsync();
                    return Ok(delivery);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return BadRequest();
        }
        [HttpPut]
        public async Task<IActionResult> Edit(DeliveryEdit deliveryEdit)
        {
            if (ModelState.IsValid)
            {
                var old_delivery = await context.Deliveries.FindAsync(deliveryEdit.Id);
                if (old_delivery == null) { return NotFound(); }
                string city = LocationService.GetCityName(deliveryEdit.CityId);
                string district = "";
                if (!string.IsNullOrEmpty(deliveryEdit.DistrictId))
                {
                    district = LocationService.GetDistrictName(deliveryEdit.CityId, deliveryEdit.DistrictId);
                }
                if (!old_delivery.City.Equals(city) || !old_delivery.District.Equals(district))
                {
                    if (await IsExist(city, district)) return Ok(new ErrorStatus { Code = "11", Message = "Đã có địa điểm này" });
                }

                old_delivery.EstimatedDeliveryTime = deliveryEdit.EstimatedDeliveryTime;
                old_delivery.City = city;
                old_delivery.District = district;
                old_delivery.ShippingFee = deliveryEdit.ShippingFee;


                try
                {
                    context.Entry(old_delivery).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return Ok(old_delivery);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var delivery = await context.Deliveries.FindAsync(id);
            if (delivery == null) return NotFound();
            try
            {
                context.Entry(delivery).State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok(delivery);
        }
        private async Task<bool> IsExist(string cityName, string districtName)
        {
            if (!string.IsNullOrEmpty(districtName))
            {
                var d = await context.Deliveries.FirstOrDefaultAsync(dl => dl.City.Equals(cityName) && dl.District.Equals(districtName));
                return d != null;
            }
            var dl = await context.Deliveries.FirstOrDefaultAsync(dl => dl.City.Equals(cityName));
            return dl != null;

        }
    }
}
