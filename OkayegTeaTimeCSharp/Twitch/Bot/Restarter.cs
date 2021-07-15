using HLE.Time;
using System;
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

        private List<int> _hoursOfDay;

        public Restarter()
        {
            InitializePingRestart();
        }

        public Restarter(List<int> hoursOfDay) : this()
        {
            _hoursOfDay = hoursOfDay;
        }

        public void InitializeResartTimer()
        {
            InitializeResartTimer(_hoursOfDay);
        }

        public void InitializeResartTimer(List<int> hoursOfDay)
        {
            _hoursOfDay = hoursOfDay;
            Stop();
            _hoursOfDay.ForEach(h => _restartTimers.Add(new(TimeUntil(h))));
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
