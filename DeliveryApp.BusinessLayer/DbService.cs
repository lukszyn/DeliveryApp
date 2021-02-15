using DeliveryApp.DataLayer;

namespace DeliveryApp.BusinessLayer
{
    public class DbService : IDbService
    {   
        public DbService ()
        {
        }

        public void EnsureDatabaseCreation()
        {
            using (var context = new DeliveryAppDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
