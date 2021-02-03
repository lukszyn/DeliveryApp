using DeliveryApp.DataLayer.Models;
using System.Collections.Generic;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IPackagesService
    {
        public void Add(Package package);
        public ICollection<Package> GetAllPackagesToBeSend();
        public bool UpdateStatus(int id, Status status);
    }
}
