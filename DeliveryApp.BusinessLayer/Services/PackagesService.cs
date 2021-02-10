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
        public void Add(Package package)
        {
            using (var context = new DeliveryAppDbContext())
            {
                context.Packages.Add(package);
                context.SaveChanges();
            }
        }

        public ICollection<Package> GetAllPackagesToBeSend()
        {
            using (var context = new DeliveryAppDbContext())
            {
                return context.Packages
                    .Include(p => p.Sender)
                    .ThenInclude(s => s.Address)
                    .Include(p => p.ReceiverAddress)
                    .Where(p => p.Status == Status.PendingSending).ToList();
            }
        }

        public bool UpdateStatus(int packageId, int vehicleId, Status status, uint size = 0)
        {
            using (var context = new DeliveryAppDbContext())
            {
                var package = context.Packages.FirstOrDefault(p => p.Id == packageId);
                var vehicle = context.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

                if (package == null || vehicle == null)
                {
                    return false;
                }

                package.Status = status;
                vehicle.Load += size;

                context.SaveChanges();

                return true;
            }
        }

        public void ClearDriversPackagesList(ICollection<User> drivers)
        {
            using (var context = new DeliveryAppDbContext())
            {
                foreach (var driver in drivers)
                {
                    var dr = context.Users.FirstOrDefault(d => d.Id == driver.Id);
                    dr.Packages.Clear();
                    
                }
                context.SaveChanges();
            }
        }
    }
}
