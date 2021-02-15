using System.Linq;
using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer;
using DeliveryApp.DataLayer.Models;

namespace DeliveryApp.BusinessLayer.Services
{
    public class VehiclesService : IVehiclesService
    {
        public bool FindByPlate(string plate)
        {
            using (var context = new DeliveryAppDbContext())
            {
                return context.Vehicles.Any(v => v.Plate == plate);
            }
        }

        public void Add(Vehicle vehicle)
        {
            using (var context = new DeliveryAppDbContext())
            {
                context.Vehicles.Add(vehicle);
                context.SaveChanges();
            }
        }

        public bool UpdateLoad(int id, uint size)
        {
            using (var context = new DeliveryAppDbContext())
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
