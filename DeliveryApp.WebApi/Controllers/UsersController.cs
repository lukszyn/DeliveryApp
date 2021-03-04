using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.BusinessLayer.Services;
using DeliveryApp.DataLayer.Models;
using DeliveryApp.WebApi.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeliveryApp.WebApi.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IWaybillsService _waybillsService;

        public UsersController(IUsersService usersService, IWaybillsService waybillsService)
        {
            _usersService = usersService;
            _waybillsService = waybillsService;
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/users
        Body:
        {
            "id": 0,
            "firstName": "Jan",
            "lastName": "Nowak",
            "email": "jnowak@gmail.com",
            "userType": 1,
            "address": {
                "id": 0,
                "street": "aleja Grunwaldzka",
                "number": 26,
                "city": "Gdańsk",
                "zipCode": "80-226"
            }
        }
        */

        [HttpPost]
        public async Task PostUser([FromBody] User user)
        {
            await _usersService.AddAsync(user);
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/users
        Body:
        {
            "id": 0,
            "firstName": "Jan",
        }
        */

        [Route("credentials")]
        [HttpPost]
        public async Task<User> ValidateUser([FromBody] Credentials credentials)
        {
           return await _usersService.ValidateCourier(credentials.Email, credentials.Password);
        }


        /*
        Method: GET
            URI: http://localhost:10500/api/users/{id}
        */

        [Route("{id}")]
        [HttpGet]
        public async Task<User> GetUserPackages(int id)
        {
            return await _waybillsService.GetLatestWaybillAsync(id);
        }
    }
}
