using DeliveryApp.DataLayer.Models;
using System.Threading.Tasks;

namespace DeliveryApp.BusinessLayer.Interfaces
{
    public interface IVehiclesService
    {
        public bool FindByPlate(string plate);
        public Task AddAsync(Vehicle vehicle);
        public bool UpdateLoad(int id, uint size);
    }
}
