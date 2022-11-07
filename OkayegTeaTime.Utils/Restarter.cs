using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;

namespace OkayegTeaTime.Utils;

public sealed class Restarter
{
    private readonly List<Timer> _restartTimers = new();
    private readonly TimeOnly[] _restartTimes;

    public Restarter(IEnumerable<TimeOnly> restartTimes)
    {
        _restartTimes = restartTimes.ToArray();
    }

    public void Start()
    {
        Stop();
        TimeOnly now = TimeOnly.FromDateTime(DateTime.UtcNow);
        foreach (TimeOnly restartTime in _restartTimes)
        {
            double interval = (restartTime - now).TotalMilliseconds;
            Timer timer = new(interval);
            timer.Elapsed += RestartTimer_OnElapsed!;
            timer.Start();
            _restartTimers.Add(timer);
        }
    }

    public void Stop()
    {
        foreach (Timer timer in CollectionsMarshal.AsSpan(_restartTimers))
        {
            timer.Stop();
            timer.Dispose();
        }

        _restartTimers.Clear();
    }

    private static void RestartTimer_OnElapsed(object sender, ElapsedEventArgs e)
    {
        if (sender is not Timer t)
        {
            return;
        }

        t.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        ProcessUtils.Restart();
    }
}
