using DeliveryApp.BusinessLayer.Interfaces;
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

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
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
        public async Task ValidateUser([FromBody] Credentials credentials)
        {
            await _usersService.ValidateCourier(credentials.Email, credentials.Password);
        }
    }
}
