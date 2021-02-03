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
                    .Where(p => p.Status == Status.PENDING_SENDING).ToList();
            }
        }

        public bool UpdateStatus(int id, Status status)
        {
            using (var context = new DeliveryAppDbContext())
            {
                var package = context.Packages
                    .FirstOrDefault(book => book.Id == id);

                if (package == null)
                {
                    return false;
                }

                package.Status = status;
                context.SaveChanges();

                return true;
            }
        }
    }
}
