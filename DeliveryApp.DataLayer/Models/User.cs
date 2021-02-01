using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryApp.DataLayer.Models
{
    public enum UserType
    {
        Customer,
        Driver
    }

    public class User
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public Address Address { get; set;}
        public UserType UserType { get; set; }

        [InverseProperty("Sender")]
        public ICollection<Package> SentPackages { get; set; }

        [InverseProperty("Receiver")]
        public ICollection<Package> ReceivedPackages { get; set; }
    }
}
