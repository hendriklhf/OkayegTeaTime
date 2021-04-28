using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using System;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class BotHelper
    {
        public static void Send(this TwitchBot twitchBot, string channel, string message)
        {
            twitchBot.TwitchClient.SendMessage(channel.Replace("#", ""), "Okayeg " + message);
        }

        public static void SendComingBack(this TwitchBot twitchBot, ChatMessage chatMessage, User user)
        {
            throw new NotImplementedException("SendComingBack: Identify which text to send");
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            twitchBot.Send(reminder.Channel, reminder.ToUser + ", reminder from " + reminder.FromUser + " (" + TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago") + "): " + reminder.Message.ToString());
        }
    }
}
