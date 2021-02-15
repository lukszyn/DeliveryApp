using DeliveryApp.BusinessLayer;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Serializers;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;

namespace DeliveryApp
{
    class DiContainerProvider
    {
        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IIoHelper, IoHelper>();
            container.RegisterType<IMenu, Menu>();

            container.RegisterType<IGeographicDataService, GeographicDataService>();
            container.RegisterType<IPackagesService, PackagesService>();
            container.RegisterType<IUsersService, UsersService>();
            container.RegisterType<IVehiclesService, VehiclesService>();
            container.RegisterType<IWaybillsService, WaybillsService>();
            container.RegisterType<IDbService, DbService>();
            container.RegisterType<IDeliveriesService, DeliveriesService>();
            container.RegisterType<ITimersService, TimersService>();

            container.RegisterType<ISerializer, JsonSerializer>();

            container.RegisterType<Func<IDeliveryAppDbContext>>(
                new InjectionFactory(ctx => new Func<IDeliveryAppDbContext>(() => new DeliveryAppDbContext())));

            return container;
        }
    }
}
