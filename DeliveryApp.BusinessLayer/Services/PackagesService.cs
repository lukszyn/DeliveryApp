using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.BusinessLayer.Services
{
    public class PackagesService : IPackagesService
    {
        private Func<IDeliveryAppDbContext> _dbContextFactoryMethod;
        private readonly IUsersService _usersService;

        public PackagesService(Func<IDeliveryAppDbContext> dbContextFactoryMethod,
            IUsersService usersService)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
            _usersService = usersService;
        }

        public async Task AddAsync(Package package)
        {
            using (var context = _dbContextFactoryMethod())
            {
                package.Number = Guid.NewGuid();
                package.ReceiverPosition = await _usersService.GetUserPosition(package.ReceiverAddress);
                package.RegisterDate = TimeProvider.Now;
                
                context.Packages.Add(package);
                await context.SaveChangesAsync();
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
                    if (driver.Packages == null)
                    {
                        continue;
                    }

                    driver.Packages.ToList().RemoveAll(p => p.Status == Status.Delivered);
                    context.Users.Update(driver);

                }
                context.SaveChanges();
            }
        }

        public bool UpdatePackageStatus(int id, Status status)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var package = context.Packages.FirstOrDefault(p => p.Id == id);

                if (package == null)
                {
                    return false;
                }

                package.Status = status;

                if (status == Status.Delivered)
                {
                    package.DeliveryDate = TimeProvider.Now;
                }

                context.SaveChanges();

                return true;
            }
        }
    }
}
