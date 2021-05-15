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

        public static void SendRandomYourmom(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            Yourmom yourmom = DataBase.GetRandomYourmom();
            twitchBot.Send(chatMessage.Channel, $"{username}, {yourmom.MessageText} YOURMOM");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Message randomMessage = DataBase.GetRandomMessage(chatMessage);
            twitchBot.Send(chatMessage.Channel, $"({TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            Message randomMessage = DataBase.GetRandomMessage(username);
            twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username, string channel)
        {
            Message randomMessage = DataBase.GetRandomMessage(username, channel);
            twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
        }

        public static void SendLoggedMessagesCount(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountMessages()} messages across all channels");
        }

        public static void SendLoggedMessagesUserCount(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountUserMessages(username)} messages of {username}");
        }

        public static void SendLoggedMessagesChannelCount(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountChannelMessages(channel)} messages of the channel {channel}");
        }

        public static void SendFirstUserChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string username, string channel)
        {
            Message message = DataBase.GetFirstMessageUserChannel(username, channel);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendFirstChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            Message message = DataBase.GetFirstChannel(chatMessage, channel);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendFirstUser(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            Message message = DataBase.GetFirstUser(username);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendFirst(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Message message = DataBase.GetFirst(chatMessage);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendSearchUserChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string username, string channel)
        {
            Message message = DataBase.GetSearchUserChannel(keyword, username, channel);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendSearchUser(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string username)
        {
            Message message = DataBase.GetSearchUser(keyword, username);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendSearchChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string channel)
        {
            Message message = DataBase.GetSearchChannel(keyword, channel);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendSearch(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword)
        {
            Message message = DataBase.GetSearch(keyword);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendLastMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            Message message = DataBase.GetLastMessage(username);
            twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
        }

        public static void SendCoinFlip(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string result = NumberHelper.Random(0, 1) == 0 ? $"yes/heads {Emoji.Coin}" : $"no/tails {Emoji.Coin}";
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {result}");
        }

        public static void SendResumingAfkStatus(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.ResumeAfkStatus(chatMessage.Username);
            User user = DataBase.GetUser(chatMessage.Username);
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).Resuming);
        }

        public static void SendSuggestionNoted(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.AddSugestion(chatMessage, chatMessage.GetMessage()[chatMessage.GetLowerSplit()[0].Length..]);
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, your suggestion has been noted");
        }

        public static void SendCheckAfk(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            User user = DataBase.GetUser(username);
            if (user.IsAfk == "true")
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {AfkMessage.Create(user).GoingAway}: {user.MessageText.Decode()}");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {username} is not afk");
            }
        }

        public static void SendSetPrefix(this TwitchBot twitchBot, ChatMessage chatMessage, string prefix)
        {
            DataBase.SetPrefix(chatMessage.Channel, prefix);
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, prefix set to \"{prefix}\"");
        }

        public static void SendUnsetPrefix(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.UnsetPrefix(chatMessage.Channel);
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the prefix has been unset");
        }
    }
}
