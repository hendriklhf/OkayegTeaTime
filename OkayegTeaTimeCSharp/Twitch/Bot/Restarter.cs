using HLE.Time;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using static OkayegTeaTimeCSharp.Program;
using Timer = System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public class Restarter
    {
        private const int _pingInterval = 1000;

        private const string _routerIP = "192.168.178.1";

        private readonly Timer::Timer _pingTimer = new(_pingInterval);

        private readonly List<Timer::Timer> _restartTimers = new();

        private List<(int Hour, int Minute)> _restartTimes;

        public Restarter()
        {
            InitializePingRestart();
        }

        public Restarter(List<(int, int)> restartTimes) : this()
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

        private void InitializePingRestart()
        {
            _pingTimer.Elapsed += PingTimer_OnElapsed;
            _pingTimer.Start();
        }

        private void PingTimer_OnElapsed(object sender, Timer::ElapsedEventArgs e)
        {
            if (new Ping().Send(_routerIP).Status != IPStatus.Success)
            {
                Thread.Sleep(10000);
                Restart();
            }
        }

        private void RestartTimer_OnElapsed(object sender, Timer::ElapsedEventArgs e)
        {
            (sender as Timer::Timer).Interval = new Day().Milliseconds;
            Restart();
        }
    }
}
