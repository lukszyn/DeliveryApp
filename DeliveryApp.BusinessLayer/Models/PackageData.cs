using DeliveryApp.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.BusinessLayer.Models
{
    public class PackageData
    {
        public Guid Id { get; set; }
        public string Receiver { get; set; }
        public string Street { get; set; }
        public uint Number { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
    }
}
