using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Models;
using DeliveryApp.DataLayer.Models;

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
        private readonly IVehiclesService _vehiclesService;

        public DeliveriesService(IPackagesService packagesService, IUsersService usersService,
            IVehiclesService vehiclesService)
        {
            _packagesService = packagesService;
            _usersService = usersService;
            _vehiclesService = vehiclesService;
        }

        public void StartDelivering()
        {
            DeliverPackages(Status.OnTheWay);
        }

        public void FinishDelivering()
        {
            DeliverPackages(Status.Delivered);
        }

        public void DeliverPackages(Status status)
        {
            var drivers = _usersService.GetAllDrivers();

            foreach (var driver in drivers)
            {
                if (driver.ManualDelivery)
                {
                    continue;
                }

                var packages = _usersService.GetDriverPackages(driver.Id);

                if (packages == null) return;

                foreach (var package in packages)
                {
                    _packagesService.UpdateStatus(package.Id, driver.Id,
                       status);

                    if (status == Status.Delivered)
                    {
                        _vehiclesService.UpdateLoad(driver.Vehicle.Id, 0);

                        //new ConfirmationRequestsService().SendRequest(new PackageData() {
                        //    Id = package.Number,
                        //    Receiver = package.Receiver,
                        //    Street = package.ReceiverAddress.Street,
                        //    Number = package.ReceiverAddress.Number,
                        //    ZipCode = package.ReceiverAddress.ZipCode,
                        //    City = package.ReceiverAddress.City,
                        //    Email = package.Sender.Email,
                        //});
                    }
                }
            }
        }
    }
}
