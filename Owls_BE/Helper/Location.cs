using Newtonsoft.Json;

namespace Owls_BE.Helper
{
    public static class LocationService
    {
        public static List<CityData> _cities { get; set; } = new();


        public static async Task InitializeAsync()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data.json");
            var data = await File.ReadAllTextAsync(filePath);
            _cities = JsonConvert.DeserializeObject<List<CityData>>(data);
        }

        public static string GetCityName(string cityId)
        {
            var city = _cities.FirstOrDefault(c => c.Id == cityId);
            return city?.Name;
        }

        public static string GetDistrictName(string cityId, string districtId)
        {
            var city = _cities.FirstOrDefault(c => c.Id == cityId);
            var district = city?.Districts.FirstOrDefault(d => d.Id == districtId);
            return district?.Name;
        }

        public static string GetWardName(string cityId, string districtId, string wardId)
        {
            var city = _cities.FirstOrDefault(c => c.Id == cityId);
            var district = city?.Districts.FirstOrDefault(d => d.Id == districtId);
            var ward = district?.Wards.FirstOrDefault(w => w.Id == wardId);
            return ward?.Name;
        }
        public static string GetCityId(string cityName)
        {
            var city = _cities.FirstOrDefault(c => c.Name == cityName);
            return city?.Id;
        }

        public static string GetDistrictId(string cityName, string districtName)
        {
            var city = _cities.FirstOrDefault(c => c.Name == cityName);
            var district = city?.Districts.FirstOrDefault(d => d.Name == districtName);
            return district?.Id;
        }

        public static string GetWardId(string cityName, string districtName, string wardName)
        {
            var city = _cities.FirstOrDefault(c => c.Name == cityName);
            var district = city?.Districts.FirstOrDefault(d => d.Name == districtName);
            var ward = district?.Wards.FirstOrDefault(w => w.Name == wardName);
            return ward?.Id;
        }
    }





    public class CityData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<DistrictData> Districts { get; set; }
    }

    public class DistrictData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<WardData> Wards { get; set; }
    }

    public class WardData
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
