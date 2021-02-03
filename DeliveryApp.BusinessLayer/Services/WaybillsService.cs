using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Serializers;
using DeliveryApp.DataLayer.Models;
using GeoCoordinatePortable;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeliveryApp.BusinessLayer.Services
{
    public class WaybillsService : IWaybillsService
    {
        private readonly IPackagesService _packagesService;
        private readonly IUsersService _usersService;
        private readonly IGeographicDataService _geoDataService;
        private readonly IVehiclesService _vehiclesService;
        private readonly ISerializer _serializer;

        public WaybillsService(IPackagesService packagesService, IUsersService usersService, IVehiclesService vehiclesService,
                                ISerializer serializer)
        {
            _packagesService = packagesService;
            _usersService = usersService;
            _vehiclesService = vehiclesService;
            _serializer = serializer;
            _geoDataService = new GeographicDataService();
        }

        public void MatchPackages()
        {
            var packages = _packagesService.GetAllPackagesToBeSend();

            if (packages is null) return;

            var drivers = _usersService.GetAllDrivers().ToList();

            foreach (var package in packages)
            {
                //dla kazdej paczki szukam kuriera ktory bedzie musial pokonac najmniejsza odleglosc
                var closestDriver = ChooseDriver(package, drivers);
                //po dobraniu kuriera zmieniam status paczki i dodaje ja do paczek kuriera, zmieniam tez oblozenie
                //samochodu kuriera
                if (closestDriver != null)
                {
                    _packagesService.UpdateStatus(package.Id, Status.SENT);

                    _vehiclesService.UpdateOccupancy(closestDriver.Vehicle.Id, (uint)package.Size);

                    closestDriver.Packages.Add(package);
                }
            }
            //generuje listy przewozowe dla kazdego kuriera na dany dzien
            GenerateWaybills(drivers);

        }

        private void GenerateWaybills(List<User> drivers)
        {
            var path = @"shipping_lists";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var driver in drivers)
            {
                var fileName = $"{path}\\{driver.Id}_{AcceleratedDateTime.Now.ToShortDateString()}.json";
                _serializer.Serialize(fileName, driver);
            }
        }

        public User ChooseDriver(Package package, List<User> drivers)
        {
            User closestDriver = null;
            double closestDist = double.MaxValue;

            var senderLocation = GetLocation(package.Sender.Address); //pozycja nadawcy
            var receiverLocation = GetLocation(package.ReceiverAddress); //pozycja odbiorcy paczki

            var distanceSendToRec = senderLocation.GetDistanceTo(receiverLocation); //odleglosc miedzy nimi

            foreach (var driver in drivers)
            {
                //szukam kuriera ktory bedzie musial pokonac najmniejsza odleglosc miedzy swoim punktem poczatkowym,
                //nadawca, odbiorca i z powrotem swoim punktem
                var driverLocation = GetLocation(driver.Address);
                var dist = driverLocation.GetDistanceTo(senderLocation)
                         + distanceSendToRec + driverLocation.GetDistanceTo(receiverLocation);

                //sprawdzam dodatkowo czy w samochodzie kuriera jest miejsce na paczke
                if (dist < closestDist &&
                    (driver.Vehicle.Occupancy + (uint)package.Size < driver.Vehicle.Capacity))
                {
                    closestDriver = driver;
                    closestDist = dist;
                }
            }
            return closestDriver;
        }

        public GeoCoordinate GetLocation(Address address)
        {
            var country = "Poland";
            var city = address.City;
            var street = address.Street;
            var building = address.Number.ToString();

            var data = _geoDataService.GetCoordinatesForAddress(country, city, street, building)[0];

            return new GeoCoordinate(data.Lat, data.Lon);
        }
    }
}
