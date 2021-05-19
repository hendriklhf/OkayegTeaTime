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
        }

        private static void CreateTimer(int interval)
        {
            Timer timer = new()
            {
                Enabled = false,
                Interval = interval,
                AutoReset = true,
            };
            TwitchBot.ListTimer.Add(timer);
        }

        public static Timer GetTimer(int interval)
        {
            return TwitchBot.ListTimer.Where(timer => timer.Interval == interval).FirstOrDefault();
        }
    }
}