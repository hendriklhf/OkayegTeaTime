using HLE.Collections;
using HLE.Emojis;
using HLE.HttpRequests;
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
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Spotify;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using StrbhRand = HLE.Randoms;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class BotActions
    {
        public static void AddAfkCooldown(string username)
        {
            if (TwitchBot.AfkCooldowns.Any(c => c.Username == username))
            {
                TwitchBot.AfkCooldowns.Remove(TwitchBot.AfkCooldowns.FirstOrDefault(c => c.Username == username));
                AddUserToAfkCooldownDictionary(username);
            }
        }

        public static void AddCooldown(string username, CommandType type)
        {
            if (TwitchBot.Cooldowns.Any(c => c.Username == username && c.Type == type))
            {
                TwitchBot.Cooldowns.Remove(TwitchBot.Cooldowns.FirstOrDefault(c => c.Username == username && c.Type == type));
                AddUserToCooldownDictionary(username, type);
            }
        }

        public static void AddUserToAfkCooldownDictionary(string username)
        {
            if (!TwitchConfig.Moderators.Contains(username))
            {
                if (!TwitchBot.AfkCooldowns.Any(c => c.Username == username))
                {
                    TwitchBot.AfkCooldowns.Add(new AfkCooldown(username));
                }
            }
        }

        public static void AddUserToCooldownDictionary(string username, CommandType type)
        {
            if (!TwitchConfig.Moderators.Contains(username))
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

        public static string Send7TVEmotes(ITwitchChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.LowerSplit.Length > 2)
                {
                    emotes = HttpRequest.Get7TVEmotes(chatMessage.Channel, chatMessage.Split[2].ToInt());
                }
                else
                {
                    emotes = HttpRequest.Get7TVEmotes(chatMessage.Channel);
                }
                string emoteString = string.Empty;
                emotes.ForEach(e => emoteString += $"{e} | ");
                return $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}";
            }
            catch (Exception)
            {
                return $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled";
            }
        }

        public static string SendBTTVEmotes(ITwitchChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.LowerSplit.Length > 2)
                {
                    emotes = HttpRequest.GetBTTVEmotes(chatMessage.Channel, chatMessage.Split[2].ToInt());
                }
                else
                {
                    emotes = HttpRequest.GetBTTVEmotes(chatMessage.Channel);
                }
                string emoteString = string.Empty;
                emotes.ForEach(e => emoteString += $"{e} | ");
                return $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}";
            }
            catch (Exception)
            {
                return $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled";
            }
        }

        public static string SendChatterino2Links()
        {
            return $"Website: chatterino.com || Releases: github.com/Chatterino/chatterino2/releases";
        }

        public static string SendChatterino7Links()
        {
            return $"Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases";
        }

        public static string SendChattersCount(ITwitchChatMessage chatMessage)
        {
            string channel = chatMessage.LowerSplit.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Channel;
            DottedNumber chatterCount = HttpRequest.GetChatterCount(channel);
            if (chatterCount > 1)
            {
                return $"{chatMessage.Username}, there are {chatterCount} chatters in the channel of {channel}";
            }
            else if (chatterCount > 0)
            {
                return $"{chatMessage.Username}, there is {chatterCount} chatter in the channel of {channel}";
            }
            else
            {
                return $"{chatMessage.Username}, there are no chatters in the channel of {channel}";
            }
        }

        public static string SendCheckAfk(IChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.LowerSplit[2];
                User user = DataBase.GetUser(username);
                if (user.IsAfk == true)
                {
                    string message = $"{chatMessage.Username}, {AfkMessage.Create(user).GoingAway}";
                    message += user.MessageText.Decode().Length > 0 ? $": {user.MessageText.Decode()} ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})" : $" ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})";
                    return message;
                }
                else
                {
                    return $"{chatMessage.Username}, {username} is not afk";
                }
            }
            catch (UserNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendCheckMessage(IChatMessage chatMessage)
        {
            try
            {
                int id = chatMessage.Split[2].ToInt();
                Message message = DataBase.GetMessage(id);
                return $"{chatMessage.Username}, ({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendCheckReminder(IChatMessage chatMessage)
        {
            try
            {
                int id = chatMessage.Split[2].ToInt();
                Reminder reminder = DataBase.GetReminder(id);
                return $"{chatMessage.Username}, From: {GetReminderAuthor(reminder.ToUser, reminder.FromUser)} || To: {GetReminderTarget(reminder.ToUser, reminder.FromUser)} || " +
                    $"Set: {TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago", ConversionType.YearDayHourMin)} || " +
                    (reminder.ToTime > 0 ? $"Fires in: {TimeHelper.ConvertUnixTimeToTimeStamp(reminder.ToTime, conversionType: ConversionType.YearDayHourMin)} || " : string.Empty) +
                    $"Message: {reminder.Message.Decode()}";
            }
            catch (ReminderNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendCoinFlip(IChatMessage chatMessage)
        {
            string result = StrbhRand.Random.Int(0, 100) >= 50 ? "yes/heads" : "no/tails";
            return $"{chatMessage.Username}, {result} {Emoji.Coin}";
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ITwitchChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static string SendCompilerResult(IChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {HttpRequest.GetOnlineCompilerResult(chatMessage.Message[(chatMessage.Split[0].Length + 1)..])}";
        }

        public static string SendCreatedNuke(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                string word = chatMessage.LowerSplit[1];
                long timeoutTime = TimeHelper.ConvertStringToSeconds(new() { chatMessage.LowerSplit[2] });
                long duration = TimeHelper.ConvertTimeToMilliseconds(new() { chatMessage.LowerSplit[3] });
                timeoutTime = timeoutTime > new Week(2).Seconds ? new Week(2).Seconds : timeoutTime;
                int id = DataBase.AddNuke(new(chatMessage.Username, $"#{chatMessage.Channel}", word.MakeInsertable(), timeoutTime, duration + TimeHelper.Now()));
                return $"{chatMessage.Username}, timeouting \"{word}\" {chatMessage.LowerSplit[2]} for the next {chatMessage.LowerSplit[3]} (ID: {id})";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendDetectedSpotifyURI(IChatMessage chatMessage)
        {
            if (new LinkRecognizer(chatMessage).FindSpotifyLink(out string uri))
            {
                return $"{uri}";
            }
            else
            {
                return null;
            }
        }

        public static string SendFFZEmotes(ITwitchChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.LowerSplit.Length > 2)
                {
                    emotes = HttpRequest.GetFFZEmotes(chatMessage.Channel, chatMessage.Split[2].ToInt());
                }
                else
                {
                    emotes = HttpRequest.GetFFZEmotes(chatMessage.Channel);
                }
                string emoteString = string.Empty;
                emotes.ForEach(e => emoteString += $"{e} | ");
                return $"{chatMessage.Username}, recently added emotes: {emoteString.Trim()[..^2]}";
            }
            catch (Exception)
            {
                return $"{chatMessage.Username}, the channel doesn't have the specified amount of emotes enabled";
            }
        }

        public static string SendFill(IChatMessage chatMessage)
        {
            string message = string.Empty;
            string[] emotes = chatMessage.Split[1..];
            message += emotes.Random();
            for (; ; )
            {
                string emote = emotes.Random();
                if ($"{message} {emote}".Length <= TwitchConfig.MaxMessageLength)
                {
                    message += $" {emote}";
                }
                else
                {
                    break;
                }
            }
            return message;
        }

        public static string SendFirst(ITwitchChatMessage chatMessage)
        {
            try
            {
                Message message = DataBase.GetFirst(chatMessage);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFirstChannel(ITwitchChatMessage chatMessage)
        {
            try
            {
                string channel = chatMessage.LowerSplit[1];
                Message message = DataBase.GetFirstChannel(chatMessage, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFirstUser(ITwitchChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.LowerSplit[1];
                Message message = DataBase.GetFirstUser(username);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFirstUserChannel(ITwitchChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.LowerSplit[1];
                string channel = chatMessage.LowerSplit[2];
                Message message = DataBase.GetFirstMessageUserChannel(username, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFuck(IChatMessage chatMessage)
        {
            string message = $"{Emoji.PointRight} {Emoji.OkHand} {chatMessage.Username} fucked {chatMessage.Split[1]}";
            if (chatMessage.Split.Length > 2)
            {
                message += $" {chatMessage.Split[2]}";
            }
            return message;
        }

        public static void SendGoingAfk(this TwitchBot twitchBot, ITwitchChatMessage chatMessage, AfkCommandType type)
        {
            DataBase.SetAfk(chatMessage, type);
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(DataBase.GetUser(chatMessage.Username)).GoingAway);
        }

        public static string SendHelp(IChatMessage chatMessage)
        {
            string username = chatMessage.Split.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
            return $"{Emoji.PointRight} {username}, here you can find a list of commands and the repository: {Resources.GitHubRepoLink}";
        }

        public static string SendJoinChannel(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
        {
            if (TwitchConfig.Moderators.Contains(chatMessage.Username))
            {
                string channel = chatMessage.LowerSplit[1];
                string response = twitchBot.JoinChannel(channel.RemoveHashtag());
                return $"{chatMessage.Username}, {response}";
            }
            else
            {
                return $"{chatMessage.Username}, you are not a moderator of the bot";
            }
        }

        public static string SendLastMessage(ITwitchChatMessage chatMessage)
        {
            try
            {
                Message message = DataBase.GetLastMessage(chatMessage.LowerSplit[1]);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago")}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (UserNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendLoggedMessagesChannelCount(ITwitchChatMessage chatMessage)
        {
            string channel = chatMessage.LowerSplit[1];
            return $"{chatMessage.Username}, logging {new DottedNumber(new OkayegTeaTimeContext().CountChannelMessages(channel))} messages of the channel {channel}";
        }

        public static string SendLoggedMessagesCount(ITwitchChatMessage chatMessage)
        {
            DottedNumber dMessageCount = new OkayegTeaTimeContext().CountMessages();
            return $"{chatMessage.Username}, logging {dMessageCount} messages across all channels";
        }

        public static string SendLoggedMessagesUserCount(ITwitchChatMessage chatMessage)
        {
            string username = chatMessage.LowerSplit[1];
            return $"{chatMessage.Username}, logging {new DottedNumber(new OkayegTeaTimeContext().CountUserMessages(username))} messages of {username}";
        }

        public static string SendMassping(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                string emote = chatMessage.Split.Length > 1 ? chatMessage.Split[1] : EmoteDictionary.Get(chatMessage.Channel);
                string message = string.Empty;
                List<string> chatters;
                List<string> chattersToRemove = new(TwitchConfig.SpecialUsers) { chatMessage.Username };

                if (chatMessage.Channel != Resources.SecretOfflineChat)
                {
                    chatters = HttpRequest.GetChatters(chatMessage.Channel).Select(c => c.Username).ToList();
                    chattersToRemove.ForEach(c => chatters.Remove(c));
                    if (chatters.Count == 0)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    message = $"OkayegTeaTime {emote}";
                    chatters = Resources.SecretOfflineChatEmotes.Split().ToList();
                }

                chatters.ForEach(c => message += $" {c} {emote}");
                return message;
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendMathResult(IChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {HttpRequest.GetMathResult(chatMessage.Message[(chatMessage.Split[0].Length + 1)..])}";
        }

        public static string SendPick(IChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {chatMessage.Split[1..].Random()}";
        }

        public static string SendPing(TwitchBot twitchBot)
        {
            return $"Pongeg, I'm here! {twitchBot.GetSystemInfo()}";
        }

        public static string SendRandomCookie(IChatMessage chatMessage)
        {
            Pechkekse keks = DataBase.GetRandomCookie();
            return $"{chatMessage.Username}, {keks.Message} {Emoji.Cookie}";
        }

        public static string SendRandomGachi()
        {
            Gachi gachi = DataBase.GetRandomGachi();
            return $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS";
        }

        public static string SendRandomMessage(ITwitchChatMessage chatMessage)
        {
            try
            {
                if (chatMessage.Split.Length > 2)
                {
                    Message message = DataBase.GetRandomMessage(chatMessage.LowerSplit[1], chatMessage.LowerSplit[2].RemoveHashtag());
                    return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
                }
                else if (chatMessage.Split.Length > 1)
                {
                    Message message = DataBase.GetRandomMessage(chatMessage.LowerSplit[1]);
                    return $"({TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
                }
                else
                {
                    Message message = DataBase.GetRandomMessage(chatMessage);
                    return $"({TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
                }
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendRandomYourmom(IChatMessage chatMessage)
        {
            Yourmom yourmom = DataBase.GetRandomYourmom();
            string target = chatMessage.LowerSplit.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
            return $"{target}, {yourmom.MessageText} YOURMOM";
        }

        public static void SendReminder(this TwitchBot twitchBot, ITwitchChatMessage chatMessage, List<Reminder> reminders)
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
                reminders.Skip(1).ForEach(r =>
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

        public static string SendResumingAfkStatus(IChatMessage chatMessage)
        {
            DataBase.ResumeAfkStatus(chatMessage.Username);
            User user = DataBase.GetUser(chatMessage.Username);
            return AfkMessage.Create(user).Resuming;
        }

        public static string SendSearch(IChatMessage chatMessage, string keyword)
        {
            try
            {
                Message message = DataBase.GetSearch(keyword);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSearchChannel(IChatMessage chatMessage, string keyword, string channel)
        {
            try
            {
                Message message = DataBase.GetSearchChannel(keyword, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSearchUser(IChatMessage chatMessage, string keyword, string username)
        {
            try
            {
                Message message = DataBase.GetSearchUser(keyword, username);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSearchUserChannel(IChatMessage chatMessage, string keyword, string username, string channel)
        {
            try
            {
                Message message = DataBase.GetSearchUserChannel(keyword, username, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSetEmoteInFront(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                string emote = chatMessage.Split[2][..(chatMessage.Split[2].Length > TwitchConfig.MaxEmoteInFrontLength ? TwitchConfig.MaxEmoteInFrontLength : chatMessage.Split[2].Length)];
                DataBase.SetEmoteInFront(chatMessage.Channel, emote);
                EmoteDictionary.Update(chatMessage.Channel);
                return $"{chatMessage.Username}, emote set to: {emote}";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendSetPrefix(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                string prefix = chatMessage.LowerSplit[2][..(chatMessage.LowerSplit[2].Length > TwitchConfig.MaxPrefixLength ? TwitchConfig.MaxPrefixLength : chatMessage.LowerSplit[2].Length)];
                DataBase.SetPrefix(chatMessage.Channel, prefix);
                PrefixDictionary.Update(chatMessage.Channel);
                return $"{chatMessage.Username}, prefix set to: {prefix}";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendSetReminder(ITwitchChatMessage chatMessage, byte[] message)
        {
            try
            {
                string target = chatMessage.LowerSplit[1] == "me" ? chatMessage.Username : chatMessage.LowerSplit[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}"));
                return $"{chatMessage.Username}, set a reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})";
            }
            catch (TooManyReminderException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSetSongRequestState(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                OkayegTeaTimeContext database = new();
                if (database.Spotify.Any(s => s.Username == chatMessage.Channel))
                {
                    bool state = chatMessage.Split[2].IsMatch(@"(1|true|enabled?)");
                    database.Spotify.Where(s => s.Username == chatMessage.Channel.RemoveHashtag()).FirstOrDefault().SongRequestEnabled = state;
                    database.SaveChanges();
                    return $"{chatMessage.Username}, song requests {(state ? "enabled" : "disabled")} for channel {chatMessage.Channel}";
                }
                else
                {
                    return $"{chatMessage.Username}, channel {chatMessage.Channel} is not registered, they have to register first";
                }
            }
            else
            {
                return $"{chatMessage.Username}, you have to be a mod or the broadcaster to set song request settings";
            }
        }

        public static string SendSetTimedReminder(ITwitchChatMessage chatMessage, byte[] message, long toTime)
        {
            try
            {
                string target = chatMessage.LowerSplit[1] == "me" ? chatMessage.Username : chatMessage.LowerSplit[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}", toTime + TimeHelper.Now()));
                return $"{chatMessage.Username}, set a timed reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})";
            }
            catch (TooManyReminderException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSongAddedToQueue(ITwitchChatMessage chatMessage)
        {
            string song = chatMessage.Split[1];
            return $"{chatMessage.Username}, {SpotifyRequest.AddToQueue(chatMessage.Channel, song).Result}";
        }

        public static string SendSongSkipped(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                return $"{chatMessage.Username}, {SpotifyRequest.SkipToNextSong(chatMessage.Channel).Result}";
            }
            else
            {
                return $"{chatMessage.Username}, you have to be a mod or the broadcaster to skip the song";
            }
        }

        public static string SendSpotifyCurrentlyPlaying(ITwitchChatMessage chatMessage)
        {
            string username = chatMessage.LowerSplit.Length > 1 ? (chatMessage.LowerSplit[1] == "me" ? chatMessage.Username : chatMessage.LowerSplit[1]) : chatMessage.Channel;
            return $"{chatMessage.Username}, {SpotifyRequest.GetCurrentlyPlaying(username).Result}";
        }

        public static string SendSpotifySearch(IChatMessage chatMessage)
        {
            string query = chatMessage.Split.Skip(2).ToSequence();
            return $"{chatMessage.Username}, {SpotifyRequest.Search(query).Result}";
        }

        public static string SendSuggestionNoted(ITwitchChatMessage chatMessage)
        {
            DataBase.AddSugestion(chatMessage, chatMessage.Message[chatMessage.LowerSplit[0].Length..]);
            return $"{chatMessage.Username}, your suggestion has been noted";
        }

        public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
        {
            if (reminder.Message.Length > 0)
            {
                twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {GetReminderAuthor(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")}): {reminder.Message.Decode()}");
            }
            else
            {
                twitchBot.Send(reminder.Channel, $"{reminder.ToUser}, reminder from {GetReminderAuthor(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")})");
            }
        }

        public static string SendTuckedToBed(IChatMessage chatMessage)
        {
            string target = chatMessage.LowerSplit[1];
            string emote = chatMessage.LowerSplit.Length > 2 ? chatMessage.Split[2] : string.Empty;
            return $"{Emoji.PointRight} {Emoji.Bed} {chatMessage.Username} tucked {target} to bed {emote}".Trim();
        }

        public static string SendUnsetEmoteInFront(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                DataBase.UnsetEmoteInFront(chatMessage.Channel);
                EmoteDictionary.Update(chatMessage.Channel);
                return $"{chatMessage.Username}, unset emote";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendUnsetNuke(ITwitchChatMessage chatMessage)
        {
            try
            {
                DataBase.RemoveNuke(chatMessage);
                return $"{chatMessage.Username}, the nuke has been unset";
            }
            catch (NukeNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
            catch (NoPermissionException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendUnsetPrefix(ITwitchChatMessage chatMessage)
        {
            if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
            {
                DataBase.UnsetPrefix(chatMessage.Channel);
                PrefixDictionary.Update(chatMessage.Channel);
                return $"{chatMessage.Username}, the prefix has been unset";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendUnsetReminder(ITwitchChatMessage chatMessage)
        {
            try
            {
                DataBase.RemoveReminder(chatMessage);
                return $"{chatMessage.Username}, the reminder has been unset";
            }
            catch (NoPermissionException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
            catch (ReminderNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendUserID(IChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.Split.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
                return $"{chatMessage.Username}, {new TwitchAPI().GetChannelID(username)}";
            }
            catch (UserNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }
        public static void Timeout(this TwitchBot twitchBot, string channel, string username, long time, string reason = "")
        {
            twitchBot.TwitchClient.SendMessage(channel, $"/timeout {username} {time} {reason}".Trim());
        }

        public static void SendBanFromFile(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
        {
            try
            {
                if (TwitchConfig.Moderators.Contains(chatMessage.Username))
                {
                    List<string> fileContent = new HttpGet(chatMessage.Split[1]).Result.Split("\n").ToList();
                    string regex = chatMessage.Split[2];
                    fileContent.Where(f => f.IsMatch(regex)).ForEach(f =>
                    {
                        if (f.IsMatch(@"^[\./]ban\s\w+"))
                        {
                            twitchBot.TwitchClient.SendMessage(chatMessage.Channel, f);
                        }
                        else
                        {
                            twitchBot.TwitchClient.SendMessage(chatMessage.Channel, $"/ban {f}");
                        }
                    });
                }
                else
                {
                    twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, you must be a moderator of the bot");
                }
            }
            catch (Exception)
            {
                twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, something went wrong");
            }
        }
    }
}
