using System.Linq;
using System.Timers;
using HLE.Time;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class Timers
    {
        public static void CreateTimers()
        {
            CreateTimer(new Second().Milliseconds);
            CreateTimer(new Second(30).Milliseconds);
            CreateTimer(new Minute().Milliseconds);
            CreateTimer(new Day(10).Milliseconds);
        }

        private static void CreateTimer(long interval)
        {
            Timer timer = new()
            {
                Enabled = false,
                Interval = interval,
                AutoReset = true
            };
            TwitchBot.Timers.Add(timer);
        }

        public static Timer GetTimer(long interval)
        {
            return TwitchBot.Timers.FirstOrDefault(t => t.Interval == interval);
        }
    }
}