using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.DataLayer.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public int Capacity { get; set; }
        public User Driver { get; set; }
    }
}
