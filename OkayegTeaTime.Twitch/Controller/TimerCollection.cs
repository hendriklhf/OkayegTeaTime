using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class TimerCollection : IEnumerable<Timer>
{
    private readonly List<Timer> _timers = new();

    public void Add(Func<object?, ElapsedEventArgs, ValueTask> action, double interval, bool autoReset = true, bool startDirectly = true)
    {
        Timer? timer = _timers.FirstOrDefault(t => Math.Abs(t.Interval - interval) <= 0 && t.AutoReset == autoReset);
        if (timer is null)
        {
            timer = new(interval)
            {
                AutoReset = autoReset,
                Enabled = startDirectly
            };
            _timers.Add(timer);
        }

        timer.Elapsed += async (sender, e) => await action(sender, e);
        if (!timer.Enabled)
        {
            timer.Start();
        }
    }

    public void StartAll()
    {
        foreach (Timer timer in CollectionsMarshal.AsSpan(_timers))
        {
            timer.Start();
        }
    }

    public void StopAll()
    {
        foreach (Timer timer in CollectionsMarshal.AsSpan(_timers))
        {
            timer.Stop();
        }
    }

    public IEnumerator<Timer> GetEnumerator()
    {
        return _timers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
