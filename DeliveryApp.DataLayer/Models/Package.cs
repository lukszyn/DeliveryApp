using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryApp.DataLayer.Models
{
    public enum Size
    {
        Small = 15,
        Medium = 50,
        Large = 150
    }

    public enum Status
    {
        PendingSending,
        Sent,
        OnTheWay,
        PendingDelivery,
        Delivered
    }

    public class Package
    {
        public int Id { get; set; }
        public Guid Number { get; set; }
        [ForeignKey("User")]
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public string Receiver { get; set; }
        public Address ReceiverAddress { get; set; }
        public Position ReceiverPosition { get; set; }
        public DateTime RegisterDate { get; set; }
        public Size Size { get; set; }
        public Status Status { get; set; }
    }
}
