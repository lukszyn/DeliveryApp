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
                                ISerializer serializer, IGeographicDataService geoDataService)
        {
            _packagesService = packagesService;
            _usersService = usersService;
            _vehiclesService = vehiclesService;
            _serializer = serializer;
            _geoDataService = geoDataService;
        }

        public void MatchPackages()
        {
            var packages = _packagesService.GetAllPackagesToBeSend();

            if (!packages.Any())
            {
                return;
            }

            var drivers = _usersService.GetAllDrivers().ToList();
            _packagesService.ClearDriversPackagesList(drivers);

            foreach (var package in packages)
            {
                var closestDriver = ChooseDriver(package, drivers);

                if (closestDriver != null)
                {
                    _packagesService.UpdateStatus(package.Id,
                                                  closestDriver.Id,
                                                  Status.Sent,
                                                  (uint)package.Size);

                    closestDriver.Packages.Add(package);

                    _usersService.UpdatePackages(closestDriver.Id, package);
                }
            }

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
                var fileName = $"{path}\\{driver.Id}_{TimeProvider.Now.ToShortDateString()}.json";
                _serializer.Serialize(fileName, driver);
            }
        }

        public User ChooseDriver(Package package, List<User> drivers)
        {
            User closestDriver = null;
            double closestDist = double.MaxValue;
            double workingHours = 10.0d;

            var senderLocation = new GeoCoordinate(package.Sender.Position.Latitude, package.Sender.Position.Longitude);
            var receiverLocation = new GeoCoordinate(package.ReceiverPosition.Latitude, package.ReceiverPosition.Longitude);

            var distanceSendToRec = senderLocation.GetDistanceTo(receiverLocation);

            foreach (var driver in drivers)
            {
                var driverLocation = new GeoCoordinate(driver.Position.Latitude, driver.Position.Longitude);
                var dist = driverLocation.GetDistanceTo(senderLocation)
                         + distanceSendToRec + driverLocation.GetDistanceTo(receiverLocation);

                if (dist < closestDist &&
                    (driver.Vehicle.Load + (uint)package.Size < driver.Vehicle.Capacity))
                {
                    var deliveryTime = CalculateDeliveryTime(driver);
                    var newPackageDeliveryTime = EstimateDriveTime(driver.Vehicle.AverageSpeed,
                        driver.Position, package.Sender.Position, package.ReceiverPosition);

                    if (deliveryTime + newPackageDeliveryTime > workingHours)
                    {
                        continue;
                    }

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

        public double CalculateDeliveryTime(User driver)
        {
            var packages = _usersService.GetDriverPackages(driver.Id);
            var position = driver.Position;
            var workTime = 0.0d;

            foreach (var pckg in packages)
            {
                workTime += EstimateDriveTime(driver.Vehicle.AverageSpeed, position, pckg.Sender.Position, 
                    pckg.ReceiverPosition);
                position = pckg.ReceiverPosition;
            }

            driver.Position = position;
            return workTime;
        }

        public double EstimateDriveTime(double avgSpeed, Position current, Position next, Position final)
        {
            var start = new GeoCoordinate(current.Latitude, current.Longitude);
            var middle = new GeoCoordinate(next.Latitude, next.Longitude);
            var end = new GeoCoordinate(final.Latitude, final.Longitude);

            var dist = (start.GetDistanceTo(middle) + middle.GetDistanceTo(end)) / 1000;

            return dist / avgSpeed;
        }
    }


}
