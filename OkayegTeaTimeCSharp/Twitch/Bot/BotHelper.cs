using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotHelper
    {
        public static void Send(this TwitchBot twitchBot, string channel, string message)
        {
            twitchBot.TwitchClient.SendMessage(channel.Replace("#", ""), $"Okayeg {message}");
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user)
        {
            throw new NotImplementedException("SendComingBack: Identify which text to send");
        }

        public static void SendReminder(this TwitchBot twitchBot, string username, List<Reminder> listReminder)
        {
            string message = $"{username}, reminder from {listReminder[0].FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(listReminder[0].Time, " ago")}): {listReminder[0].Message}";
            if (listReminder.Count > 1)
            {
                listReminder.Skip(1).ToList().ForEach(reminder =>
                {
                    message += $" || {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message}";
                });
            }
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message}");
        }
    }
}
