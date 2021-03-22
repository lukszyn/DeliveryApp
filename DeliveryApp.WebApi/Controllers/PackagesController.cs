using DeliveryApp.BusinessLayer.Interfaces;
using DeliveryApp.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliveryApp.WebApi.Controllers
{
    [Route("api/packages")]
    public class PackagesController
    {
        private readonly IPackagesService _packagesService;

        public PackagesController(IPackagesService packagesService)
        {
            _packagesService = packagesService;
        }

        /*
        Method: POST
        URI: http://localhost:10500/api/packages
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
        public async Task PostPackage([FromBody] Package package)
        {
            await _packagesService.AddAsync(package);
        }

        [HttpPut]
        [Route("update")]
        public async Task UpdatePackagesStatus([FromBody] List<Package> packages)
        {
            foreach (var package in packages)
            {
                _packagesService.UpdatePackageStatus(package.Id, Status.OnTheWay);
            }
        }

        [HttpPut]
        [Route("deliver")]
        public async Task DeliverPackage([FromBody] Package package)
        {
            _packagesService.UpdatePackageStatus(package.Id, Status.Delivered);
        }
    }
}