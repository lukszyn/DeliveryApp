using DeliveryApp.DataLayer;
using System;

namespace DeliveryApp.BusinessLayer
{
    public class DbService : IDbService
    {   
        private Func<IDeliveryAppDbContext> _dbContextFactoryMethod;

        public DbService(Func<IDeliveryAppDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public void EnsureDatabaseCreation()
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
