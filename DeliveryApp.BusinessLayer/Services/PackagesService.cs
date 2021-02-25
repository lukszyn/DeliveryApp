using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.BusinessLayer.Services
{
    public class PackagesService : IPackagesService
    {
        private Func<IDeliveryAppDbContext> _dbContextFactoryMethod;

        public PackagesService(Func<IDeliveryAppDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public void Add(Package package)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Packages.Add(package);
                context.SaveChanges();
            }
        }

        public ICollection<Package> GetAllPackagesToBeSend()
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Packages
                    .Include(p => p.Sender)
                    .ThenInclude(s => s.Address)
                    .Include(p => p.Sender)
                    .ThenInclude(s => s.Position)
                    .Include(p => p.ReceiverAddress)
                    .Include(p => p.ReceiverPosition)
                    .Where(p => p.Status == Status.PendingSending).ToList();
            }
        }

        public bool UpdateStatus(int packageId, int driverId, Status status, uint loadToAdd = 0)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var package = context.Packages.FirstOrDefault(p => p.Id == packageId);
                var driver = context.Users.Include(u => u.Vehicle).FirstOrDefault(u => u.Id == driverId);
                var vehicle = context.Vehicles.FirstOrDefault(v => v.Id == driver.Vehicle.Id);

                if (package == null || vehicle == null)
                {
                    return false;
                }

                if (package.Courier == null)
                {
                    package.Courier = driver;
                }
                
                package.Status = status;
                vehicle.Load += loadToAdd;

                context.SaveChanges();

                return true;
            }
        }

        public void ClearDriversPackagesList(ICollection<User> drivers)
        {
            using (var context = _dbContextFactoryMethod())
            {
                foreach (var driver in drivers)
                {
                    var dr = context.Users
                        .Include(u => u.Packages)
                        .FirstOrDefault(d => d.Id == driver.Id);

                    if (dr.Packages == null)
                    {
                        continue;
                    }

                    dr.Packages.ToList().RemoveAll(p => p.Status == Status.Delivered);
                    context.Users.Update(driver);

                }
                context.SaveChanges();
            }
        }
    }
}
