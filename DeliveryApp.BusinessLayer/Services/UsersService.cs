using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;

namespace DeliveryApp.BusinessLayer.Services
{
    public class UsersService : IUsersService
    {
        public bool FindByEmail(string email)
        {
            using (var context = new DeliveryAppDbContext())
            {
                return context.Users.Any(user => user.Email == email);
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
    }
}
