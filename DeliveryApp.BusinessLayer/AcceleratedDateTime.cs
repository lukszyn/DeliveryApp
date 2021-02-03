using System;

namespace DeliveryApp.BusinessLayer
{
    public static class AcceleratedDateTime
    {
        private static DateTime StartTime { get; set; } = new DateTime(2020, 11, 3, 0, 0, 0);

        public static DateTime Now {
            get
            {
                return StartTime.AddSeconds((DateTime.Now - StartTime).TotalSeconds * 60);
            }
        }

    }
}
