using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Exceptions;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotActions
    {
        public static void AddAfkCooldown(string username)
        {
            if (TwitchBot.ListAfkCooldowns.Any(c => c.Username == username))
            {
                TwitchBot.ListAfkCooldowns.Remove(
                    TwitchBot.ListAfkCooldowns.Where(c => c.Username == username).FirstOrDefault()
                    );
                AddUserToAfkCooldownDictionary(username);
            }
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

        public static void AddUserToAfkCooldownDictionary(string username)
        {
            if (username != Resources.Owner)
            {
                if (!TwitchBot.ListAfkCooldowns.Any(c => c.Username == username))
                {
                    TwitchBot.ListAfkCooldowns.Add(new AfkCooldown(username));
                }
            }
        }

        public static void AddUserToCooldownDictionary(string username, CommandType type)
        {
            if (username != Resources.Owner)
            {
                if (!TwitchBot.ListCooldowns.Any(c => c.Username == username && c.Type == type))
                {
                    TwitchBot.ListCooldowns.Add(new Cooldown(username, type));
                }
            }
        }

        public static void FillLastMessagesDictionary()
        {
            Config.GetChannels().ForEach(channel =>
            {
                TwitchBot.LastMessages.Add($"#{channel}", "");
            });
        }

        public static string GetReminderAuthor(string toUser, string fromUser)
        {
            return toUser == fromUser ? "yourself" : fromUser;
        }

        public static bool IsOnAfkCooldown(string username)
        {
            return TwitchBot.ListAfkCooldowns.Any(c => c.Username == username && c.Time > TimeHelper.Now());
        }

        public static bool IsOnCooldown(string username, CommandType type)
        {
            return TwitchBot.ListCooldowns.Any(c => c.Username == username && c.Type == type && c.Time > TimeHelper.Now());
        }

        public static void SendChattersCount(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, there are {HttpRequest.GetChatterCount(channel)} chatter in the channel of {channel}");
        }

        public static void SendCheckAfk(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            try
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
            catch (UserNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendCoinFlip(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string result = NumberHelper.Random(0, 1) == 0 ? "yes/heads" : "no/tails";
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {result} {Emoji.Coin}");
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static void SendCreatedNuke(this TwitchBot twitchBot, ChatMessage chatMessage, string word, long timeoutTime, long duration)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                timeoutTime = timeoutTime > Day.ToSeconds(14) ? Day.ToSeconds(14) : timeoutTime;
                DataBase.AddNuke(new Nuke(chatMessage.Username, $"#{chatMessage.Channel}", word.MakeInsertable(), timeoutTime, duration));
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, timeouting '{word}' {chatMessage.GetLowerSplit()[2]} for the next {chatMessage.GetLowerSplit()[3]}");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendFirst(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                Message message = DataBase.GetFirst(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendFirstChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            try
            {
                Message message = DataBase.GetFirstChannel(chatMessage, channel);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendFirstUser(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            try
            {
                Message message = DataBase.GetFirstUser(username);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendFirstUserChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string username, string channel)
        {
            try
            {
                Message message = DataBase.GetFirstMessageUserChannel(username, channel);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendGoingAfk(this TwitchBot twitchBot, ChatMessage chatMessage, AfkCommandType type)
        {
            DataBase.SetAfk(chatMessage, type);
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(DataBase.GetUser(chatMessage.Username)).GoingAway);
        }

        public static void SendLastMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            try
            {
                Message message = DataBase.GetLastMessage(username);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (UserNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendLoggedMessagesChannelCount(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {database.CountChannelMessages(channel)} messages of the channel {channel}");
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

        public static void SendRandomCookie(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Pechkekse keks = DataBase.GetRandomCookie();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {keks.Message}");
        }

        public static void SendRandomGachi(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Gachi gachi = DataBase.GetRandomGachi();
            twitchBot.Send(chatMessage.Channel, $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS");
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                Message randomMessage = DataBase.GetRandomMessage(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"({TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            try
            {
                Message randomMessage = DataBase.GetRandomMessage(username);
                twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendRandomMessage(this TwitchBot twitchBot, ChatMessage chatMessage, string username, string channel)
        {
            try
            {
                Message randomMessage = DataBase.GetRandomMessage(username, channel);
                twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(randomMessage.Time, " ago")}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
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

        public static void SendReminder(this TwitchBot twitchBot, ChatMessage chatMessage, List<Reminder> listReminder)
        {
            string message = $"{chatMessage.Username}, reminder from {GetReminderAuthor(chatMessage.Username, listReminder[0].FromUser)} ({TimeHelper.ConvertMillisecondsToPassedTime(listReminder[0].Time, " ago")}): {listReminder[0].Message.Decode()}";
            if (listReminder.Count > 1)
            {
                listReminder.Skip(1).ToList().ForEach(reminder =>
                {
                    message += $" || {GetReminderAuthor(chatMessage.Username, reminder.FromUser)} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message.Decode()}";
                });
            }
            twitchBot.Send(chatMessage.Channel, message);
        }

        public static void SendResumingAfkStatus(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.ResumeAfkStatus(chatMessage.Username);
            User user = DataBase.GetUser(chatMessage.Username);
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).Resuming);
        }

        public static void SendSearch(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword)
        {
            try
            {
                Message message = DataBase.GetSearch(keyword);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSearchChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string channel)
        {
            try
            {
                Message message = DataBase.GetSearchChannel(keyword, channel);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSearchUser(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string username)
        {
            try
            {
                Message message = DataBase.GetSearchUser(keyword, username);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSearchUserChannel(this TwitchBot twitchBot, ChatMessage chatMessage, string keyword, string username, string channel)
        {
            try
            {
                Message message = DataBase.GetSearchUserChannel(keyword, username, channel);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertMillisecondsToPassedTime(message.Time, " ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSetPrefix(this TwitchBot twitchBot, ChatMessage chatMessage, string prefix)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.SetPrefix(chatMessage.Channel, prefix);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, prefix set to \"{prefix}\"");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendSetReminder(this TwitchBot twitchBot, ChatMessage chatMessage, byte[] message)
        {
            string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
            int id = DataBase.AddReminder(new Reminder(chatMessage.Username, target, message, $"#{chatMessage.Channel}"));
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, set a reminder for {GetReminderAuthor(target, chatMessage.Username)} (ID: {id})");
        }

        public static void SendSetTimedReminder(this TwitchBot twitchBot, ChatMessage chatMessage, byte[] message, long toTime)
        {
            string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
            int id = DataBase.AddReminder(new Reminder(chatMessage.Username, target, message, $"#{chatMessage.Channel}", toTime));
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, set a timed reminder for {GetReminderAuthor(target, chatMessage.Username)} (ID: {id})");
        }

        public static void SendSuggestionNoted(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.AddSugestion(chatMessage, chatMessage.GetMessage()[chatMessage.GetLowerSplit()[0].Length..]);
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, your suggestion has been noted");
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {GetReminderAuthor(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertMillisecondsToPassedTime(reminder.Time, " ago")}): {reminder.Message.Decode()}");
        }

        public static void SendUnsetPrefix(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.UnsetPrefix(chatMessage.Channel);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the prefix has been unset");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendUnsetReminder(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                DataBase.UnsetReminder(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the reminder has been unset");
            }
            catch (ReminderNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void Timeout(this TwitchBot twitchBot, string channel, string username, long time, string reason = "")
        {
            twitchBot.TwitchClient.SendMessage(channel, $"/timeout {username} {time} {reason}".Trim());
        }
    }
}