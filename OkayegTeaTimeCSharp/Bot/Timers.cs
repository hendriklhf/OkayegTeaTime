using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class Timers
    {
        public static void InitializeTimers()
        {
            Timer timerCheckForTimedReminders = new()
            {
                Enabled = false,
                Interval = 1000,
                AutoReset = true,
            };
            TwitchBot.ListTimer.Add(timerCheckForTimedReminders);
        }
    }
}
