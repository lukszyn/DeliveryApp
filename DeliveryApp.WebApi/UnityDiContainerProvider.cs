using DeliveryApp.BusinessLayer;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Serializers;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer;
using System;
using Unity;
using Unity.Injection;

namespace DeliveryApp.WebApi
{
    public class UnityDiContainerProvider
    {
        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IGeographicDataService, GeographicDataService>();
            container.RegisterType<IPackagesService, PackagesService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IVehiclesService, VehiclesService>();
            container.RegisterType<IWaybillsService, WaybillsService>();
            container.RegisterType<IDbService, DbService>();
            container.RegisterType<IDeliveriesService, DeliveriesService>();
            container.RegisterType<ITimersService, TimersService>();
            container.RegisterType<IConfirmationRequestsService, ConfirmationRequestsService>();

            container.RegisterType<ISerializer, JsonSerializer>();

            container.RegisterType<Func<IDeliveryAppDbContext>>(
                new InjectionFactory(ctx => new Func<IDeliveryAppDbContext>(() => new DeliveryAppDbContext())));

            return container;
        }
    }
}
