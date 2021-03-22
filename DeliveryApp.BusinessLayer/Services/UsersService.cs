using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<User> ValidateCourier(string email, string password)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return  await context.Users
                    .Include(u => u.Position)
                    .Include(u => u.Vehicle)
                    .FirstOrDefaultAsync(user => user.Email == email
                                         && user.Password == password
                                         && user.UserType == UserType.Driver);
            }
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

        public async Task AddAsync(User user)
        {
            using (var context = _dbContextFactoryMethod())
            {
                user.Position = await GetUserPosition(user.Address);
                context.Users.Add(user);
                await context.SaveChangesAsync();
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
                    .ThenInclude(p => p.ReceiverAddress)
                    .Include(u => u.Packages)
                    .ThenInclude(p => p.Courier)
                    .Include(u => u.Packages)
                    .ThenInclude(p => p.Sender)
                    .Include(u => u.Position)
                    .Where(u => u.UserType == UserType.Driver)
                    .ToList();
            }
        }

        public bool UpdatePackages(User user, Package package)
        {
            using (var context = _dbContextFactoryMethod())
            {
                if (user == null)
                {
                    return false;
                }

                if (user.Packages == null)
                {
                    user.Packages = new List<Package>();
                }

                user.Packages.Add(package);

                context.SaveChanges();

                return true;
            }
        }

        public async Task<Position> GetUserPosition(Address address)
        {
            var userGeoPosition = await _geoDataService.GetCoordinatesForAddress("Poland",
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

        public async Task SetDeliveryMode(int id, bool isManual)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var driver = await context.Users.AsQueryable().FirstOrDefaultAsync(u => u.Id == id);

                driver.ManualDelivery = isManual;

                await context.SaveChangesAsync();
            }
        }
    }
}
