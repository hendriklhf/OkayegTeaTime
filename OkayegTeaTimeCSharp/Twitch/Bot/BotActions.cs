using HLE.Emojis;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTimeCSharp.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Exceptions;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Spotify;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using StrbhRand = HLE.Randoms;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotActions
    {
        public static void AddAfkCooldown(string username)
        {
            if (TwitchBot.AfkCooldowns.Any(c => c.Username == username))
            {
                TwitchBot.AfkCooldowns.Remove(
                    TwitchBot.AfkCooldowns.Where(c => c.Username == username).FirstOrDefault()
                    );
                AddUserToAfkCooldownDictionary(username);
            }
        }

        public static void AddCooldown(string username, CommandType type)
        {
            if (TwitchBot.Cooldowns.Any(c => c.Username == username && c.Type == type))
            {
                TwitchBot.Cooldowns.Remove(
                    TwitchBot.Cooldowns.Where(c => c.Username == username && c.Type == type).FirstOrDefault()
                    );
                AddUserToCooldownDictionary(username, type);
            }
        }

        public static void AddUserToAfkCooldownDictionary(string username)
        {
            if (username != Resources.Owner)
            {
                if (!TwitchBot.AfkCooldowns.Any(c => c.Username == username))
                {
                    TwitchBot.AfkCooldowns.Add(new AfkCooldown(username));
                }
            }
        }

        public static void AddUserToCooldownDictionary(string username, CommandType type)
        {
            if (username != Resources.Owner)
            {
                if (!TwitchBot.Cooldowns.Any(c => c.Username == username && c.Type == type))
                {
                    TwitchBot.Cooldowns.Add(new Cooldown(username, type));
                }
            }
        }

        public static string GetReminderAuthor(string toUser, string fromUser)
        {
            return toUser == fromUser ? "yourself" : fromUser;
        }

        public static string GetReminderTarget(string toUser, string fromUser)
        {
            return toUser == fromUser ? "yourself" : toUser;
        }

        public static bool IsOnAfkCooldown(string username)
        {
            return TwitchBot.AfkCooldowns.Any(c => c.Username == username && c.Time > TimeHelper.Now());
        }

        public static bool IsOnCooldown(string username, CommandType type)
        {
            return TwitchBot.Cooldowns.Any(c => c.Username == username && c.Type == type && c.Time > TimeHelper.Now());
        }

        public static void Send7TVEmotes(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length >= 4)
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.Get7TVEmotes(chatMessage.GetLowerSplit()[2], chatMessage.GetLowerSplit()[3].ToInt());
                    }
                    else
                    {
                        emotes = HttpRequest.Get7TVEmotes(chatMessage.Channel, chatMessage.GetLowerSplit()[3].ToInt());
                    }
                }
                else
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.Get7TVEmotes(chatMessage.GetLowerSplit()[2]);
                    }
                    else
                    {
                        emotes = HttpRequest.Get7TVEmotes(chatMessage.Channel);
                    }
                }
                string emoteString = string.Empty;
                emotes.ForEach(e =>
                {
                    emoteString += $"{e} | ";
                });
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}");
            }
            catch (Exception)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled");
            }
        }

        public static void SendBTTVEmotes(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length >= 4)
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.GetBTTVEmotes(chatMessage.GetLowerSplit()[2], chatMessage.GetLowerSplit()[3].ToInt());
                    }
                    else
                    {
                        emotes = HttpRequest.GetBTTVEmotes(chatMessage.Channel, chatMessage.GetLowerSplit()[3].ToInt());
                    }
                }
                else
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.GetBTTVEmotes(chatMessage.GetLowerSplit()[2]);
                    }
                    else
                    {
                        emotes = HttpRequest.GetBTTVEmotes(chatMessage.Channel);
                    }
                }
                string emoteString = string.Empty;
                emotes.ForEach(e =>
                {
                    emoteString += $"{e} | ";
                });
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}");
            }
            catch (Exception)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled");
            }
        }

        public static void SendChatNeighbours(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            List<string> chatters = HttpRequest.GetChatters(chatMessage.Channel).OrderByDescending(chatters => chatters).ToList();
            if (chatters.Count >= 3 && (chatters.IndexOf(chatMessage.Username) != 0 || chatters.IndexOf(chatMessage.Username) != chatters.Count - 1))
            {
                string chatterLeft = chatters[chatters.IndexOf(chatMessage.Username) - 1];
                string chatterRight = chatters[chatters.IndexOf(chatMessage.Username) + 1];
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, your chatneighbours are {chatterLeft} and {chatterRight}");
            }
            else
            {
#warning needs else
            }
        }

        public static void SendChattersCount(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            int chatterCount = HttpRequest.GetChatterCount(channel);
            if (chatterCount > 1)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, there are {new DottedNumber(chatterCount)} chatters in the channel of {channel}");
            }
            else if (chatterCount > 0)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, there is {new DottedNumber(chatterCount)} chatter in the channel of {channel}");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, there are no chatters in the channel of {channel}");
            }
        }

        public static void SendCheckAfk(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            try
            {
                User user = DataBase.GetUser(username);
                if (user.IsAfk == "true")
                {
                    string message = $"{chatMessage.Username}, {AfkMessage.Create(user).GoingAway}";
                    message += user.MessageText.Decode().Length > 0 ? $": {user.MessageText.Decode()} ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})" : $" ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})";
                    twitchBot.Send(chatMessage.Channel, message);
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
            string result = StrbhRand.Random.Int(0, 100) >= 50 ? "yes/heads" : "no/tails";
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {result} {Emoji.Coin}");
        }

        public static void SendColor(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
#warning needs implementation
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static void SendCompilerResult(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {HttpRequest.GetOnlineCompilerResult(chatMessage.GetMessage()[(chatMessage.GetSplit()[0].Length + 1)..])}");
        }

        public static void SendCreatedNuke(this TwitchBot twitchBot, ChatMessage chatMessage, string word, long timeoutTime, long duration)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                timeoutTime = timeoutTime > new Week(2).Seconds ? new Week(2).Seconds : timeoutTime;
                int id = DataBase.AddNuke(new(chatMessage.Username, $"#{chatMessage.Channel}", word.MakeInsertable(), timeoutTime, duration + TimeHelper.Now()));
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, timeouting '{word}' {chatMessage.GetLowerSplit()[2]} for the next {chatMessage.GetLowerSplit()[3]} (ID: {id})");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendDetectedSpotifyURI(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            (bool isMatch, string uri) = new LinkRecognizer(chatMessage).FindSpotifyLink();
            if (isMatch)
            {
                twitchBot.Send(chatMessage.Channel, $"{uri}");
            }
        }

        public static void SendFFZEmotes(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length >= 4)
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.GetFFZEmotes(chatMessage.GetLowerSplit()[2], chatMessage.GetLowerSplit()[3].ToInt());
                    }
                    else
                    {
                        emotes = HttpRequest.GetFFZEmotes(chatMessage.Channel, chatMessage.GetLowerSplit()[3].ToInt());
                    }
                }
                else
                {
                    if (chatMessage.GetLowerSplit()[2].IsMatch(@"\w+"))
                    {
                        emotes = HttpRequest.GetFFZEmotes(chatMessage.GetLowerSplit()[2]);
                    }
                    else
                    {
                        emotes = HttpRequest.GetFFZEmotes(chatMessage.Channel);
                    }
                }
                string emoteString = string.Empty;
                emotes.ForEach(e =>
                {
                    emoteString += $"{e} | ";
                });
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}");
            }
            catch (Exception)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled");
            }
        }

        public static void SendFill(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string message = string.Empty;
            if (chatMessage.GetSplit()[1].IsMatch(@"rand(om)?"))
            {
                message += (char)StrbhRand::Random.Int(0, ushort.MaxValue);
                while (message.Length + 2 <= Config.MaxMessageLength)
                {
                    message += $" {(char)StrbhRand.Random.Int(0, ushort.MaxValue)}";
                }
            }
            else
            {
                string[] emotes = chatMessage.GetMessage()[(chatMessage.GetSplit()[0].Length + 1)..].Split();
                message += emotes[StrbhRand.Random.Int(0, emotes.Length - 1)];
                while (true)
                {
                    string emote = emotes[StrbhRand::Random.Int(0, emotes.Length - 1)];
                    if ($"{message} {emote}".Length <= Config.MaxMessageLength)
                    {
                        message += $" {emote}";
                    }
                    else
                    {
                        break;
                    }
                }
            }
            twitchBot.TwitchClient.SendMessage(chatMessage.Channel, message);
        }

        public static void SendFirst(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                Message message = DataBase.GetFirst(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendFuck(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string message = $"{Emoji.PointRight} {Emoji.OkHand} {chatMessage.Username} fucked {chatMessage.GetSplit()[1]}";
            if (chatMessage.GetSplit().Length > 2)
            {
                message += $" {chatMessage.GetSplit()[2]}";
            }
            twitchBot.Send(chatMessage.Channel, message);
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago")}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (UserNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendLoggedMessagesChannelCount(this TwitchBot twitchBot, ChatMessage chatMessage, string channel)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {new DottedNumber(database.CountChannelMessages(channel))} messages of the channel {channel}");
        }

        public static void SendLoggedMessagesCount(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {new DottedNumber(database.CountMessages())} messages across all channels");
        }

        public static void SendLoggedMessagesUserCount(this TwitchBot twitchBot, ChatMessage chatMessage, string username)
        {
            OkayegTeaTimeContext database = new();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, logging {new DottedNumber(database.CountUserMessages(username))} messages of {username}");
        }

        public static void SendMassping(this TwitchBot twitchBot, ChatMessage chatMessage, string emote = "Okayeg")
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                string message = emote;
                List<string> chatters;

                if (chatMessage.Channel != Resources.SecretOfflineChat)
                {
                    chatters = HttpRequest.GetChatters(chatMessage.Channel);
                    chatters.Remove(chatMessage.Username);
                }
                else
                {
                    message = $"{emote} OkayegTeaTime {emote}";
                    chatters = Resources.SecretOfflineChatEmotes.Split().ToList();
                }

                chatters.ForEach(c =>
                {
                    message += $" {c} {emote}";
                });
                twitchBot.TwitchClient.SendMessage(chatMessage.Channel, message);
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendMathResult(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {HttpRequest.GetMathResult(chatMessage.GetMessage()[(chatMessage.GetSplit()[0].Length + 1)..])}");
        }

        public static void SendRandomCookie(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            Pechkekse keks = DataBase.GetRandomCookie();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {keks.Message} {Emoji.Cookie}");
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
                twitchBot.Send(chatMessage.Channel, $"({TimeHelper.ConvertUnixTimeToTimeStamp(randomMessage.Time, "ago", ConversionType.YearDayHour)}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(randomMessage.Time, "ago", ConversionType.YearDayHour)}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({randomMessage.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(randomMessage.Time, "ago", ConversionType.YearDayHour)}) {randomMessage.Username}: {randomMessage.MessageText.Decode()}");
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

        public static void SendReminder(this TwitchBot twitchBot, ChatMessage chatMessage, List<Reminder> reminders)
        {
            string message;
            if (reminders[0].Message.Length > 0)
            {
                message = $"{chatMessage.Username}, reminder from {GetReminderAuthor(chatMessage.Username, reminders[0].FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminders[0].Time, "ago")}): {reminders[0].Message.Decode()}";
            }
            else
            {
                message = $"{chatMessage.Username}, reminder from {GetReminderAuthor(chatMessage.Username, reminders[0].FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminders[0].Time, "ago")})";
            }
            if (reminders.Count > 1)
            {
                reminders.Skip(1).ToList().ForEach(r =>
                {
                    if (r.Message.Length > 0)
                    {
                        message += $" || {GetReminderAuthor(chatMessage.Username, r.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(r.Time, "ago")}): {r.Message.Decode()}";
                    }
                    else
                    {
                        message += $" || {GetReminderAuthor(chatMessage.Username, r.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(r.Time, "ago")})";
                    }
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
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
                twitchBot.Send(chatMessage.Channel, $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}");
            }
            catch (MessageNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSetEmoteInFront(this TwitchBot twitchBot, ChatMessage chatMessage, string emote)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.SetEmoteInFront(chatMessage.Channel, emote);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, emote set to: {emote}");
                EmoteInFrontHelper.Update(chatMessage.Channel, emote);
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendSetPrefix(this TwitchBot twitchBot, ChatMessage chatMessage, string prefix)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.SetPrefix(chatMessage.Channel, prefix);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, prefix set to: {prefix}");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendSetReminder(this TwitchBot twitchBot, ChatMessage chatMessage, byte[] message)
        {
            try
            {
                string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}"));
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, set a reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})");
            }
            catch (TooManyReminderException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSetSongRequestState(this TwitchBot twitchBot, ChatMessage chatMessage, bool state)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                OkayegTeaTimeContext database = new();
                if (database.Spotify.Any(s => s.Username == chatMessage.Channel))
                {
                    database.Spotify.Where(s => s.Username == chatMessage.Channel.RemoveHashtag()).FirstOrDefault().SongRequestEnabled = state;
                    database.SaveChanges();
                    twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, song requests {(state ? "enabled" : "disabled")} for channel {chatMessage.Channel}");
                }
                else
                {
                    twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, channel {chatMessage.Channel} is not registered, they have to register first");
                }
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you have to be a mod or the broadcaster to set song request settings");
            }
        }

        public static void SendSetTimedReminder(this TwitchBot twitchBot, ChatMessage chatMessage, byte[] message, long toTime)
        {
            try
            {
                string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}", toTime + TimeHelper.Now()));
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, set a timed reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})");
            }
            catch (TooManyReminderException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
        }

        public static void SendSongAddedToQueue(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string song = chatMessage.GetSplit()[1];
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {SpotifyRequest.AddToQueue(chatMessage.Channel, song).Result}");
        }

        public static void SendSongSkipped(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {SpotifyRequest.SkipToNextSong(chatMessage.Channel).Result}");
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you have to be a mod or the broadcaster to skip the song");
            }
        }

        public static void SendSpotifyCurrentlyPlaying(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string username = chatMessage.GetLowerSplit().Length > 1 ? (chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1]) : chatMessage.Channel;
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {SpotifyRequest.GetCurrentlyPlaying(username).Result}");
        }

        public static void SendSpotifySearch(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            string query = chatMessage.GetSplit().Skip(2).ToArray().ArrayToString();
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {SpotifyRequest.Search(query).Result}");
        }

        public static void SendSuggestionNoted(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.AddSugestion(chatMessage, chatMessage.GetMessage()[chatMessage.GetLowerSplit()[0].Length..]);
            twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, your suggestion has been noted");
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            if (reminder.Message.Length > 0)
            {
                twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {GetReminderTarget(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")}): {reminder.Message.Decode()}");
            }
            else
            {
                twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {GetReminderTarget(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")})");
            }
        }

        public static void SendUnsetEmoteInFront(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.UnsetEmoteInFront(chatMessage.Channel);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, unset emote");
                EmoteInFrontHelper.Update(chatMessage.Channel, null);
            }
            else
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you aren't a mod or the broadcaster");
            }
        }

        public static void SendUnsetNuke(this TwitchBot twitchBot, ChatMessage chatMessage)
        {
            try
            {
                DataBase.RemoveNuke(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the nuke has been unset");
            }
            catch (NukeNotFoundException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
            catch (NoPermissionException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
            }
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
                DataBase.RemoveReminder(chatMessage);
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, the reminder has been unset");
            }
            catch (NoPermissionException ex)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, {ex.Message}");
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