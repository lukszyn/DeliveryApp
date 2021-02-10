using System;
using System.Timers;

namespace DeliveryApp.BusinessLayer
{
    public interface ITimeProvider
    {
        void OnTimedEvent(object source, ElapsedEventArgs e);
        void SetTimer(Action action, DateTime start);
    }

    public class TimeProvider : ITimeProvider

    {
        private const double TimeMultiplier = 60.0d;
        private static DateTime StartTime { get; set; } = new DateTime(2020, 11, 3, 0, 0, 0);
        private Timer _timer;
        private Action _timeElapsed;
        private DateTime _timerStart;

        public static DateTime Now
        {
            get
            {
                return StartTime.AddSeconds((DateTime.Now - StartTime).TotalSeconds * TimeMultiplier);
            }
        }

        public void SetTimer(Action action, DateTime start)
        {
            var nowTime = Now;

            if (nowTime > start)
            {
                start = start.AddDays(1);
            }

            double tickTime = (double)(start - Now).TotalSeconds;
            _timer = new Timer(tickTime);

            _timeElapsed = action;
            _timerStart = start;
            _timer.Elapsed += OnTimedEvent;

            _timer.Start();
            _timer.Enabled = true;
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            _timeElapsed?.Invoke();
            _timer.Stop();
            SetTimer(_timeElapsed, _timerStart);
        }
    }

}
