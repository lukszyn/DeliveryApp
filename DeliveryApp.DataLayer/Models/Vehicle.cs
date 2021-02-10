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
        public uint Capacity { get; set; }
        public uint Load { get; set; }
        public int AverageSpeed { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
