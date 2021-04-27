using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Bot;
using OkayegTeaTimeCSharp.Database.Models;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class BotHelper
    {
        public static void Send(this TwitchBot twitchBot, string channel, string message)
        {
            twitchBot.TwitchClient.SendMessage(channel.Replace("#", ""), "Okayeg " + message);
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {

        }
    }
}
