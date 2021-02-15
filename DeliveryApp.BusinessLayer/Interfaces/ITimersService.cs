using System;
using System.Timers;

namespace DeliveryApp.BusinessLayer.Services
{
    public interface ITimersService
    {
        void OnTimedEvent(object source, ElapsedEventArgs e);
        void SetTimer(Action action, DateTime start);
    }
}