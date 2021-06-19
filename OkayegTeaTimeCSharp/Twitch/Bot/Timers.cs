using Sterbehilfe.Time;
using System.Linq;
using System.Timers;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class Timers
    {
        public static void CreateTimers()
        {
            CreateTimer(1000);
            CreateTimer(30000);
            CreateTimer(new Day(10).ToMilliseconds());
        }

        private static void CreateTimer(long interval)
        {
            Timer timer = new()
            {
                Enabled = false,
                Interval = interval,
                AutoReset = true,
            };
            TwitchBot.ListTimer.Add(timer);
        }

        public static Timer GetTimer(long interval)
        {
            return TwitchBot.ListTimer.Where(timer => timer.Interval == interval).FirstOrDefault();
        }
    }
}