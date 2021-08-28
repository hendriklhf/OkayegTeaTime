using HLE.Time;
using System.Collections.Generic;
using static OkayegTeaTimeCSharp.Program;
using Timer = System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class Restarter
    {

        private readonly List<Timer::Timer> _restartTimers = new();

        private List<(int Hour, int Minute)> _restartTimes;

        public Restarter(List<(int, int)> restartTimes)
        {
            _restartTimes = restartTimes;
        }

        public void InitializeResartTimer()
        {
            InitializeResartTimer(_restartTimes);
        }

        public void InitializeResartTimer(List<(int, int)> hoursOfDay)
        {
            _restartTimes = hoursOfDay;
            Stop();
            _restartTimes.ForEach(r => _restartTimers.Add(new(TimeHelper.MillisecondsUntil(r.Hour, r.Minute))));
            _restartTimers.ForEach(t =>
            {
                t.Elapsed += RestartTimer_OnElapsed;
                t.Start();
            });
        }

        public void Stop()
        {
            _restartTimers.ForEach(t =>
            {
                t.Stop();
                t.Dispose();
            });
            _restartTimers.Clear();
        }

        private void RestartTimer_OnElapsed(object sender, Timer::ElapsedEventArgs e)
        {
            (sender as Timer::Timer).Interval = new Day().Milliseconds;
            Restart();
        }
    }
}
