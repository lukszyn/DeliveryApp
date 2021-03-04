using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeliveryApp.DataLayer.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public uint Number { get; set; }
        public string City { get; set; }

        [RegularExpression(@"\d{2}-\d{3}")]
        public string ZipCode { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
