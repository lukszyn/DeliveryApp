using DeliveryApp.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IUsersService
    {
        public bool FindByEmail(string email);
        public void Add(User user);
    }
}
