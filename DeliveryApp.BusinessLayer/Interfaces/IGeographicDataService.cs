using DeliveryApp.BusinessLayer.Serializers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IGeographicDataService
    {
        public GeoResponse GetAddressForCoordinates(double latitude, double longitude);
        public Task<GeoResponse> GetCoordinatesForAddress(string country, string city, string street, string building);
    }
}
