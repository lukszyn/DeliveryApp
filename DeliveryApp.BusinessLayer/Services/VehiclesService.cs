using System;
using System.Linq;
using System.Threading.Tasks;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;

namespace DeliveryApp.BusinessLayer.Services
{
    public class VehiclesService : IVehiclesService
    {
        private Func<IDeliveryAppDbContext> _dbContextFactoryMethod;

        public VehiclesService(Func<IDeliveryAppDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public bool FindByPlate(string plate)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Vehicles.Any(v => v.Plate == plate);
            }
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Vehicles.Add(vehicle);
                await context.SaveChangesAsync();
            }
        }

        public bool UpdateLoad(int id, uint size)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var vehicle = context.Vehicles.FirstOrDefault(v => v.Id == id);

                if (vehicle == null)
                {
                    return false;
                }
                vehicle.Load = size;

                context.SaveChanges();

                return true;
            }
            
        }
    }
}
