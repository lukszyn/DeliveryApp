using DeliveryApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IPackagesService
    {
        public ICollection<Package> GetAllPackagesToBeSend();
        public bool UpdateStatus(int packageId, int driverId, Status status, uint size = 0);
        public void ClearDriversPackagesList(ICollection<User> drivers);
        Task AddAsync(Package package);
        bool UpdatePackageStatus(int id, Status status);
    }
}
