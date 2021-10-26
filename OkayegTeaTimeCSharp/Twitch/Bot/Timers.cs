using System.Timers;
using HLE.Time;

namespace OkayegTeaTimeCSharp.Twitch.Bot;

public static class Timers
{
    private static readonly long[] _timerIntervals =
    {
            new Second().Milliseconds,
            new Second(30).Milliseconds,
            new Minute().Milliseconds,
            new Day(10).Milliseconds
        };

    public static void CreateTimers()
    {
        foreach (long interval in _timerIntervals)
        {
            CreateTimer(interval);
        }
    }

    private static void CreateTimer(long interval)
    {
        Timer timer = new(interval)
        {
            Enabled = false,
            AutoReset = true
        };
        TwitchBot.Timers.Add(timer);
    }

    public static Timer GetTimer(long interval)
    {
        return TwitchBot.Timers.FirstOrDefault(t => t.Interval == interval);
    }
}
