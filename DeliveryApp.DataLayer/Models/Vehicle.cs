using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
