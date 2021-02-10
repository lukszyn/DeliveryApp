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
        public bool CheckIfUserExists(string email)
        {
            using (var context = new DeliveryAppDbContext())
            {
                return context.Users.Any(user => user.Email == email);
            }
        }

        public bool CheckIfValidCourier(int id)
        {
            using (var context = new DeliveryAppDbContext())
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
            using (var context = new DeliveryAppDbContext())
            {
                return context.Users.FirstOrDefault(user => user.Email == email);
            }
        }

        public int GetUserId(string email)
        {
            using (var context = new DeliveryAppDbContext())
            {
                var user = context.Users.FirstOrDefault(user => user.Email == email);

                return user?.Id ?? 0;
            }
        }

        public void Add(User user)
        {
            using (var context = new DeliveryAppDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public ICollection<User> GetAllDrivers()
        {
            using (var context = new DeliveryAppDbContext())
            {
                return context.Users
                    .Include(u => u.Vehicle)
                    .Include(u => u.Address)
                    .Include(u => u.Packages)
                    .Where(u => u.UserType == UserType.Driver).ToList();
            }
        }

        public bool UpdatePackages(int userId, Package package)
        {
            using (var context = new DeliveryAppDbContext())
            {
                var courier = context.Users.FirstOrDefault(c => c.Id == userId);

                if (courier == null)
                {
                    return false;
                }

                courier.Packages.Add(package);

                context.SaveChanges();

                return true;
            }
        }
    }
}
