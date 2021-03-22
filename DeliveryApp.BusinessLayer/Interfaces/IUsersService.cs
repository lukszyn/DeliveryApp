using DeliveryApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IUsersService
    {
        bool CheckIfUserExists(string email);
        bool CheckIfValidCourier(int id);
        User FindUserByEmail(string email);
        int GetUserId(string email);
        ICollection<User> GetAllDrivers();
        Task AddAsync(User user);
        bool UpdatePackages(User user, Package package);
        Task<Position> GetUserPosition(Address address);
        List<Package> GetDriverPackages(int driverId);
        Task<User> ValidateCourier(string email, string password);
        Task SetDeliveryMode(int id, bool isManual);
    }
}
