using DeliveryApp.DataLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IUsersService
    {
        public bool CheckIfUserExists(string email);
        public bool CheckIfValidCourier(int id);
        public User FindUserByEmail(string email);
        public int GetUserId(string email);
        public ICollection<User> GetAllDrivers();
        public Task AddAsync(User user);
        public bool UpdatePackages(int userId, Package package);
        public Task<Position> GetUserPosition(Address address);
        List<Package> GetDriverPackages(int driverId);
        Task<bool> ValidateCourier(string email, string password);
    }
}
