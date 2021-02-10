using DeliveryApp.DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.BusinessLayer
{
    public class DbService : IDbService
    {
        public void EnsureDatabaseCreation()
        {
            using (var context = new DeliveryAppDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
