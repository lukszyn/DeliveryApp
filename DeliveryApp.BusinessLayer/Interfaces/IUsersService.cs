using DeliveryApp.DataLayer.Models;
using System.Collections.Generic;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IUsersService
    {
        public bool CheckIfUserExists(string email);
        public bool CheckIfValidCourier(int id);
        public User FindUserByEmail(string email);
        public int GetUserId(string email);
        public ICollection<User> GetAllDrivers();
        public void Add(User user);
        public bool UpdatePackages(int userId, Package package);
        Position GetUserPosition(Address address);
        List<Package> GetDriverPackages(int driverId);
    }
}
