using System.Collections.Generic;
using System.Linq;
using HLE.Collections;
using HLE.Time;
using Timer = System.Timers;

namespace OkayegTeaTime.Utils;

public class Restarter
{
    private readonly List<Timer::Timer> _restartTimers = new();

    private (int Hour, int Minute)[] _restartTimes;

    public Restarter(IEnumerable<(int, int)> restartTimes)
    {
        _restartTimes = restartTimes.ToArray();
    }

    public void Initialize()
    {
        Initialize(_restartTimes);
    }

    public void Initialize(IEnumerable<(int, int)> hoursOfDay)
    {
        _restartTimes = hoursOfDay.ToArray();
        Stop();
        _restartTimes.ForEach(r => _restartTimers.Add(new(TimeHelper.MillisecondsUntil(r.Hour, r.Minute))));
        _restartTimers.ForEach(t =>
        {
            t.Elapsed += RestartTimer_OnElapsed!;
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

    private static void RestartTimer_OnElapsed(object sender, Timer::ElapsedEventArgs e)
    {
        (sender as Timer::Timer)!.Interval = new Day().Milliseconds;
        ProcessUtils.Restart();
    }
}
