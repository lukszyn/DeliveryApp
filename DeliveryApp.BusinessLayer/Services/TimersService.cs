using System;
using System.Timers;

namespace DeliveryApp.BusinessLayer.Services
{
    public class TimersService : ITimersService
    {
        private Timer _timer;
        private Action _timeElapsed;
        private DateTime _timerStart;

        public void SetTimer(Action action, DateTime start)
        {
            var nowTime = TimeProvider.Now;

            if (nowTime > start)
            {
                start = start.AddDays(1);
            }

            double tickTime = (double)(start - nowTime).TotalSeconds / 60.0d;
            _timer = new Timer(tickTime * 1000);

            _timeElapsed = action;
            _timerStart = start;
            _timer.Elapsed += OnTimedEvent;
            //_timer.AutoReset = true;
            _timer.Start();
            //_timer.Enabled = true;
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _timeElapsed?.Invoke();
            _timer.Stop();
            SetTimer(_timeElapsed, TimeProvider.Now);
        }
    }
}