using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer.Models;
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

        [HttpPost]
        public async Task PostUser([FromBody] User user)
        {
            //await _usersService.Add(user);
        }
    }
}
