using System;
using System.Timers;

namespace DeliveryApp.BusinessLayer.Services
{
    public class TimersService : ITimersService
    {
        private Timer _timer;
        private Action _timeElapsed;
        private DateTime _timerStart;
        private const double TimeMultiplier = 60.0d;

        public void SetTimer(Action action, DateTime start)
        {
            var nowTime = TimeProvider.Now;

            if (nowTime > start)
            {
                start = start.AddDays(1);
            }

            double tickTime = (double)(start - nowTime).TotalMilliseconds / TimeMultiplier;
            _timer = new Timer(tickTime);

            _timeElapsed = action;
            _timerStart = start;

            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.AutoReset = true;
            _timer.Start();
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _timeElapsed?.Invoke();
            _timer.Interval = (_timerStart.AddDays(1) - _timerStart).TotalMilliseconds / TimeMultiplier;
        }
    }
}