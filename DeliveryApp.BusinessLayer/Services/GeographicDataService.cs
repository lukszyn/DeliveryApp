using DeliveryApp.BusinessLayer.Interfaces;
using RestSharp;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Services
{
    public class GeographicDataService : IGeographicDataService
    {
        public void GetAddressForCoordinates(double latitude, double longitude)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var data = response.Content;
        }

        public void GetCoordinatesForAddress(string country, string city, string street, string building)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&format=json&limit=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var data = response.Content;
        }
    }
}
