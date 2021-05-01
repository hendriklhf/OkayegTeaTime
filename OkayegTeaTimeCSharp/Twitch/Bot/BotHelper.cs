using OkayegTeaTimeCSharp.Commands.AfkCommands;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotHelper
    {
        public static Dictionary<string, string> LastMessages { get; private set; } = new();

        public static void FillDictionary()
        {
            Config.GetChannels().ForEach(channel =>
            {
                LastMessages.Add($"#{channel}", "");
            });
        }

        public static void Send(this TwitchBot twitchBot, string channel, string message)
        {
            message = LastMessages[$"#{channel}"] == message ? message : message + Resources.ChatterinoChar;
            twitchBot.TwitchClient.SendMessage(channel.Replace("#", ""), $"Okayeg {message}");
            LastMessages[$"#{channel}"] = message;
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static void SendReminder(this TwitchBot twitchBot, ChatMessage chatMessage, List<Reminder> listReminder)
        {
            string message = $"{chatMessage.Username}, reminder from {listReminder[0].FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(listReminder[0].Time, " ago")}): {listReminder[0].Message}";
            if (listReminder.Count > 1)
            {
                listReminder.Skip(1).ToList().ForEach(reminder =>
                {
                    message += $" || {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message}";
                });
            }
            twitchBot.Send(chatMessage.Channel, message);
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message}");
        }

        public static void Timeout(this TwitchBot twitchBot, string channel, string username, long time, string reason = "")
        {
            twitchBot.Send(channel, $"/timeout {username} {time} {reason}".Trim());
        }
    }
}
