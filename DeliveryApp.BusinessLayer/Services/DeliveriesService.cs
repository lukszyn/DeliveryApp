using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer.Models;
using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.BusinessLayer.Services
{
    public interface IDeliveriesService
    {
        void DeliverPackages(Status status);
        void FinishDelivering();
        void StartDelivering();
    }

    public class DeliveriesService : IDeliveriesService
    {
        private readonly IPackagesService _packagesService;
        private readonly IUsersService _usersService;

        public DeliveriesService(IPackagesService packagesService, IUsersService usersService)
        {
            _packagesService = packagesService;
            _usersService = usersService;
        }

        public void StartDelivering()
        {
            DeliverPackages(Status.OnTheWay);
        }

        public void FinishDelivering()
        {
            DeliverPackages(Status.Delivered);
            ClearPackagesList();
        }

        private void ClearPackagesList()
        {
            var drivers = _usersService.GetAllDrivers();

            _packagesService.ClearDriversPackagesList(drivers);

        }

        public void DeliverPackages(Status status)
        {
            var drivers = _usersService.GetAllDrivers();

            foreach (var driver in drivers)
            {
                var packages = driver.Packages;

                foreach (var package in packages)
                {
                    _packagesService.UpdateStatus(package.Id, driver.Vehicle.Id,
                       status);
                }
            }
        }
    }
}
