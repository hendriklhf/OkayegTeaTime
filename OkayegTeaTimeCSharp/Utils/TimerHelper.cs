using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using OkayegTeaTimeCSharp.Bot;
using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class TimerHelper
    {
        public static void InitializeTimers(this TwitchBot twitchBot)
        {
            List<Timer> listTimer = new();

            Timer timerCheckForTimedReminder = new()
            {
                Interval = 1000,
                AutoReset = true,
                
            };
            timerCheckForTimedReminder.Elapsed += Database.Database.CheckForTimedReminder(twitchBot);
        }
    }
}
