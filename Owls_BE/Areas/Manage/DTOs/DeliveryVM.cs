using System.ComponentModel.DataAnnotations;

namespace Owls_BE.Areas.Manage.DTOs
{
    public class DeliveryCreate
    {
        [Required]
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        [Range(0, double.MaxValue)]
        public double? ShippingFee { get; set; }
        [Range(0, 100)]
        public int? EstimatedDeliveryTime { get; set; } = 3;
    }
    public class DeliveryEdit : DeliveryCreate
    {
        public int Id { get; set; }
    }

    public class DeliveryResponse
    {
        public int Id { get; set; }
        public string? CityId { get; set; }
        public string? DistrictId { get; set; }
        public double? ShippingFee { get; set; }
        public int? EstimatedDeliveryTime { get; set; }

    }
}
