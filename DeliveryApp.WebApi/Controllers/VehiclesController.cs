using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryApp.WebApi.Controllers
{
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesService _vehiclesService;

        public VehiclesController(IVehiclesService vehiclesService)
        {
            _vehiclesService = vehiclesService;
        }


        /*
        Method: POST
        URI: http://localhost:10500/api/vehicles
        Body:
        {
          "id": 0,
          "make": "Citroen",
          "model": "C5",
          "plate": "GD123AB",
          "capacity": 500,
          "load": 0,
          "averageSpeed": 90,
          "userId": 1
        }
        */

        [HttpPost]
        public async Task PostVehicle([FromBody] Vehicle vehicle)
        {
            await _vehiclesService.AddAsync(vehicle);
        }
    }
}
