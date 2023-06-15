using System;
using System.Threading.Tasks;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct PeriodicAction
{
    public Func<ValueTask> Action { get; }

    public TimeSpan Interval { get; }

    public PeriodicAction(Func<ValueTask> action, TimeSpan interval)
    {
        Action = action;
        Interval = interval;
    }
}
