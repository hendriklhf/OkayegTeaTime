using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace OkayegTeaTime.Utils;

public sealed class Restarter
{
    private readonly Timer[] _restartTimers;
    private readonly TimeOnly[] _restartTimes;

    public Restarter(IEnumerable<TimeOnly> restartTimes)
    {
        _restartTimes = restartTimes.ToArray();
        _restartTimers = new Timer[_restartTimes.Length];
    }

    public void Start()
    {
        Stop();
        TimeOnly now = TimeOnly.FromDateTime(DateTime.UtcNow);
        for (int i = 0; i < _restartTimes.Length; i++)
        {
            double interval = (_restartTimes[i] - now).TotalMilliseconds;
            Timer timer = new(interval);
            timer.Elapsed += RestartTimer_OnElapsed!;
            timer.Start();
            _restartTimers[i] = timer;
        }
    }

    public void Stop()
    {
        Span<Timer?> timerSpan = _restartTimers;
        for (int i = 0; i < timerSpan.Length; i++)
        {
            Timer? t = timerSpan[i];
            t?.Stop();
            t?.Dispose();
        }
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
