using DeliveryApp.DataLayer.Models;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace DeliveryApp.BusinessLayer.Services
{
    public interface IWaybillsService
    {
        User ChooseDriver(Package package, List<User> drivers);
        GeoCoordinate GetLocation(Address address);
        void MatchPackages();
    }
}