using HLE.Time;
using System;
using System.Collections.Generic;
using System.Timers;
using static OkayegTeaTimeCSharp.Program;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class RestartTimer
    {
        private List<int> _hoursOfDay;

        private readonly List<Timer> _timers = new();

        public RestartTimer(List<int> hoursOfDay)
        {
            _hoursOfDay = hoursOfDay;
        }

        public void Initialize()
        {
            Initialize(_hoursOfDay);
        }

        public void Initialize(List<int> hoursOfDay)
        {
            _hoursOfDay = hoursOfDay;
            Stop();
            _hoursOfDay.ForEach(h => _timers.Add(new(TimeUntil(h))));
            _timers.ForEach(t =>
            {
                t.Elapsed += OnElapsed;
                t.Start();
            });
        }

        public void Stop()
        {
            _timers.ForEach(t =>
            {
                t.Stop();
                t.Dispose();
            });
            _timers.Clear();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            ((Timer)sender).Interval = new Day().Milliseconds;
            Restart();
        }

        private long TimeUntil(int hourOfDay)
        {
            long result = 0;
            (int Hours, int Minutes, int Seconds) now = new(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            result += new Second(60 - now.Seconds).Milliseconds;
            now.Minutes++;
            result += new Minute(60 - now.Minutes).Milliseconds;
            now.Hours++;
            if (now.Hours > hourOfDay)
            {
                result += new Hour(24 - now.Hours + hourOfDay).Milliseconds;
            }
            else
            {
                result += new Hour(hourOfDay - now.Hours).Milliseconds;
            }
            return result;
        }
    }
}
