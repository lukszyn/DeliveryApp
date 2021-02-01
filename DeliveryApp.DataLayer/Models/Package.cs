using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryApp.DataLayer.Models
{
    public enum Size
    {
        SMALL = 15,
        MEDIUM = 50,
        LARGE = 150
    }

    public enum Status
    {
        PENDING_SENDING,
        SENT,
        ON_THE_WAY,
        PENDING_DELIVERY,
        DELIVERED
    }

    public class Package
    {
        public int Id { get; set; }
        public Guid Number { get; set; }
        public User Receiver { get; set; }
        public User Sender { get; set; }
        public DateTime RegisterDate { get; set; }
        public Size Size { get; set; }
        public Status Status { get; set; }
    }
}
