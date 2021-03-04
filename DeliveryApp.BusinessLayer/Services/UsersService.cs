using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.BusinessLayer.Services
{
    public class UsersService : IUsersService
    {
        private readonly IGeographicDataService _geoDataService;
        private readonly Func<IDeliveryAppDbContext> _dbContextFactoryMethod;

        public UsersService(IGeographicDataService geoDataService, Func<IDeliveryAppDbContext> dbContextFactoryMethod)
        {
            _geoDataService = geoDataService;
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }


        public bool CheckIfUserExists(string email)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Users.Any(user => user.Email == email);
            }
        }

        public bool CheckIfValidCourier(int id)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Users
                    .Include(u => u.Vehicle)
                    .Any(user => user.Id == id 
                              && user.UserType == UserType.Driver
                              && user.Vehicle == null);
            }
        }

        public User FindUserByEmail(string email)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Users.FirstOrDefault(user => user.Email == email);
            }
        }

        public int GetUserId(string email)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var user = context.Users.FirstOrDefault(user => user.Email == email);

                return user?.Id ?? 0;
            }
        }

        public void Add(User user)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public ICollection<User> GetAllDrivers()
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Users
                    .Include(u => u.Vehicle)
                    .Include(u => u.Address)
                    .Include(u => u.Packages)
                    .Include(u => u.Position)
                    .Where(u => u.UserType == UserType.Driver).ToList();
            }
        }

        public bool UpdatePackages(int userId, Package package)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var courier = context.Users.FirstOrDefault(c => c.Id == userId);

                if (courier == null)
                {
                    return false;
                }

                if (courier.Packages == null)
                {
                    courier.Packages = new List<Package>();
                }

                courier.Packages.Add(package);

                context.SaveChanges();

                return true;
            }
        }

        public Position GetUserPosition(Address address)
        {
            var userGeoPosition = _geoDataService.GetCoordinatesForAddress("Poland",
                address.City, address.Street, address.Number.ToString());

            return new Position()
            {
                Longitude = userGeoPosition.Lon,
                Latitude = userGeoPosition.Lat,
            };
        }

        public List<Package> GetDriverPackages(int driverId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var driver = context.Users
                    .Include(u => u.Packages)
                    .ThenInclude(p => p.ReceiverAddress)
                    .Include(u => u.Packages)
                    .ThenInclude(p => p.Sender)
                    .ThenInclude(s => s.Position)
                    .Include(u => u.Packages)
                    .ThenInclude(p => p.ReceiverPosition)
                    .FirstOrDefault(u => u.Id == driverId);

                return driver.Packages.ToList();
            }
        }
    }
}
