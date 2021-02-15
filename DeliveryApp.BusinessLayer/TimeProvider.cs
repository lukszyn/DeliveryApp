using System;
using System.Timers;

namespace DeliveryApp.BusinessLayer
{
    public class TimeProvider
    {
        private const double TimeMultiplier = 60.0d;
        private static DateTime StartTime { get; set; } = new DateTime(2020, 11, 3, 0, 0, 0);

        public static DateTime Now
        {
            get
            {
                return StartTime.AddSeconds((DateTime.Now - StartTime).TotalSeconds * TimeMultiplier);
            }
        }
    }
}
