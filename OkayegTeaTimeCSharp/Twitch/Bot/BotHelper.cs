using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotHelper
    {
        public static void FillLastMessagesDictionary()
        {
            Config.GetChannels().ForEach(channel =>
            {
                TwitchBot.LastMessages.Add($"#{channel}", "");
            });
        }

        public static void AddUserToCooldownDictionary(string username, CommandType type)
        {
            if (username != Config.Owner)
            {
                if (!TwitchBot.ListCooldowns.Any(c => c.Username == username && c.Type == type))
                {
                    TwitchBot.ListCooldowns.Add(new Cooldown(username, type));
                }
            }
        }

        public static bool IsOnCooldown(string username, CommandType type)
        {
            return TwitchBot.ListCooldowns.Any(c => c.Username == username && c.Type == type && c.Time > TimeHelper.Now());
        }

        public static void AddCooldown(string username, CommandType type)
        {
            if (TwitchBot.ListCooldowns.Any(c => c.Username == username && c.Type == type))
            {
                TwitchBot.ListCooldowns.Remove(
                    TwitchBot.ListCooldowns.Where(c => c.Username == username && c.Type == type).FirstOrDefault()
                    );
                AddUserToCooldownDictionary(username, type);
            }
        }

        public static void Send(this TwitchBot twitchBot, string channel, string message)
        {
            message = TwitchBot.LastMessages[$"#{channel}"] == message ? $"{message} {Resources.ChatterinoChar}" : message;
            twitchBot.TwitchClient.SendMessage(channel.Replace("#", ""), $"Okayeg {message}");
            TwitchBot.LastMessages[$"#{channel}"] = message;
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static void SendReminder(this TwitchBot twitchBot, ChatMessage chatMessage, List<Reminder> listReminder)
        {
            string message = $"{chatMessage.Username}, reminder from {listReminder[0].FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(listReminder[0].Time, " ago")}): {listReminder[0].Message.Decode()}";
            if (listReminder.Count > 1)
            {
                listReminder.Skip(1).ToList().ForEach(reminder =>
                {
                    message += $" || {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message.Decode()}";
                });
            }
            twitchBot.Send(chatMessage.Channel, message);
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {reminder.FromUser} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message.Decode()}");
        }

        public static void Timeout(this TwitchBot twitchBot, string channel, string username, long time, string reason = "")
        {
            twitchBot.Send(channel, $"/timeout {username} {time} {reason}".Trim());
        }

        public static void SendRandomGachi(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Gachi gachi = DataBase.GetRandomGachi();
            twitchBot.Send(chatMessage.Channel, $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS");
        }
        public static void SendRandomCookie(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Pechkekse keks = DataBase.GetRandomCookie();
            twitchBot.Send(chatMessage.Channel, $"{Emoji.PointRight} {chatMessage.Username}, {keks.Message}");
        }

        public static void SendRandomYourmom(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Yourmom yourmom = DataBase.GetRandomYourmom();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {yourmom.MessageText} YOURMOM");
        }

        public static void SendRandomYourmom(this TwitchBot twitchBot, ChatMessage chatMessage, string givenUsername)
        {
            Yourmom yourmom = DataBase.GetRandomYourmom();
            twitchBot.Send(chatMessage.Channel, $"{givenUsername}, {yourmom.MessageText} YOURMOM");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Message randomMessage = DataBase.GetRandomMessage(chatMessage);
            twitchBot.Send(chatMessage.Channel, $"({TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string givenUsername)
        {
            Message randomMessage = DataBase.GetRandomMessage(givenUsername);
            twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string givenUsername, string givenChannel)
        {
            Message randomMessage = DataBase.GetRandomMessage(givenUsername, givenChannel);
            twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendLoggedMessagesCount(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountMessages()} messages across all channels");
        }

        public static void SendLoggedMessagesUserCount(this TwitchBot twitchBot, ChatMessage chatMessage, string givenUsername)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountUserMessages(givenUsername)} messages of {givenUsername}");
        }

        public static void SendLoggedMessagesChannelCount(this TwitchBot twitchBot, ChatMessage chatMessage, string givenChannel)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountChannelMessages(givenChannel)} messages of the channel {givenChannel}");
        }

        public static void SendLoggedEmoteCount(this TwitchBot twitchBot, ChatMessage chatMessage, string givenEmote)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the emote {givenEmote} was used {database.CountEmote(givenEmote)} times");
        }

        public static void SendLoggedDistinctUsersCount(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging messages of {database.CountDistinctUsers()} different users");
        }
    }
}
