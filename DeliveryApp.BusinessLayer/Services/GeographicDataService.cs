using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Serializers;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Services
{
    public class GeographicDataService : IGeographicDataService
    {
        public GeoResponse GetAddressForCoordinates(double latitude, double longitude)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return JsonConvert.DeserializeObject<GeoResponse>(response.Content);
        }

        public async Task<GeoResponse> GetCoordinatesForAddress(string country, string city, string street, string building)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&format=json&limit=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<List<GeoResponse>>(response.Content).FirstOrDefault();
        }
    }
}
