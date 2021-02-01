using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IGeographicDataService
    {
        public void GetAddressForCoordinates(double latitude, double longitude);
        public void GetCoordinatesForAddress(string country, string city, string street, string building);
    }
}
