using DeliveryApp.DataLayer.Models;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IVehiclesService
    {
        public bool FindByPlate(string plate);
        public void Add(Vehicle vehicle);
        public bool UpdateOccupancy(int id, uint size);
    }
}
