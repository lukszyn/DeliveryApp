using DeliveryApp.BusinessLayer.Serializers;
using System.Collections.Generic;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IGeographicDataService
    {
        public GeoResponse GetAddressForCoordinates(double latitude, double longitude);
        public List<GeoResponse> GetCoordinatesForAddress(string country, string city, string street, string building);
    }
}
