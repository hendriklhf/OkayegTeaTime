using HLE.Collections;
using HLE.Emojis;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Exceptions;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Spotify;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTimeCSharp.Twitch.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch.Messages;
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
            if (username != Resources.Moderators)
            {
                if (!TwitchBot.AfkCooldowns.Any(c => c.Username == username))
                {
                    TwitchBot.AfkCooldowns.Add(new AfkCooldown(username));
                }
            }
        }

        public static void AddUserToCooldownDictionary(string username, CommandType type)
        {
            if (username != Resources.Moderators)
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

        public static string Send7TVEmotes(ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length > 2)
                {
                    emotes = HttpRequest.Get7TVEmotes(chatMessage.Channel, chatMessage.GetSplit()[2].ToInt());
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

        public static string SendBTTVEmotes(ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length > 2)
                {
                    emotes = HttpRequest.GetBTTVEmotes(chatMessage.Channel, chatMessage.GetSplit()[2].ToInt());
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

        public static string SendChattersCount(ChatMessage chatMessage)
        {
            string channel = chatMessage.GetLowerSplit().Length > 1 ? chatMessage.GetLowerSplit()[1] : chatMessage.Channel;
            int chatterCount = HttpRequest.GetChatterCount(channel);
            if (chatterCount > 1)
            {
                return $"{chatMessage.Username}, there are {new DottedNumber(chatterCount)} chatters in the channel of {channel}";
            }
            else if (chatterCount > 0)
            {
                return $"{chatMessage.Username}, there is {new DottedNumber(chatterCount)} chatter in the channel of {channel}";
            }
            else
            {
                return $"{chatMessage.Username}, there are no chatters in the channel of {channel}";
            }
        }

        public static string SendCheckAfk(ChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.GetLowerSplit()[2];
                User user = DataBase.GetUser(username);
                if (user.IsAfk == "True")
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

        public static string SendCheckReminder(ChatMessage chatMessage)
        {
            try
            {
                int id = chatMessage.GetSplit()[2].ToInt();
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

        public static string SendCoinFlip(ChatMessage chatMessage)
        {
            string result = StrbhRand.Random.Int(0, 100) >= 50 ? "yes/heads" : "no/tails";
            return $"{chatMessage.Username}, {result} {Emoji.Coin}";
        }

        public static void SendComingBack(this TwitchBot twitchBot, User user, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(user).ComingBack);
        }

        public static string SendCompilerResult(ChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {HttpRequest.GetOnlineCompilerResult(chatMessage.GetMessage()[(chatMessage.GetSplit()[0].Length + 1)..])}";
        }

        public static string SendCreatedNuke(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                string word = chatMessage.GetLowerSplit()[1];
                long timeoutTime = TimeHelper.ConvertStringToSeconds(new() { chatMessage.GetLowerSplit()[2] });
                long duration = TimeHelper.ConvertTimeToMilliseconds(new() { chatMessage.GetLowerSplit()[3] });
                timeoutTime = timeoutTime > new Week(2).Seconds ? new Week(2).Seconds : timeoutTime;
                int id = DataBase.AddNuke(new(chatMessage.Username, $"#{chatMessage.Channel}", word.MakeInsertable(), timeoutTime, duration + TimeHelper.Now()));
                return $"{chatMessage.Username}, timeouting \"{word}\" {chatMessage.GetLowerSplit()[2]} for the next {chatMessage.GetLowerSplit()[3]} (ID: {id})";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
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

        public static string SendFFZEmotes(ChatMessage chatMessage)
        {
            try
            {
                List<HttpRequests.Models.Emote> emotes;
                if (chatMessage.GetLowerSplit().Length > 2)
                {
                    emotes = HttpRequest.GetFFZEmotes(chatMessage.Channel, chatMessage.GetSplit()[2].ToInt());
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

        public static string SendFill(ChatMessage chatMessage)
        {
            string message = string.Empty;
            string[] emotes = chatMessage.GetSplit()[1..];
            message += emotes.Random();
            while (true)
            {
                string emote = emotes.Random();
                if ($"{message} {emote}".Length <= Config.MaxMessageLength)
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

        public static string SendFirst(ChatMessage chatMessage)
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

        public static string SendFirstChannel(ChatMessage chatMessage)
        {
            try
            {
                string channel = chatMessage.GetLowerSplit()[1];
                Message message = DataBase.GetFirstChannel(chatMessage, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFirstUser(ChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.GetLowerSplit()[1];
                Message message = DataBase.GetFirstUser(username);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFirstUserChannel(ChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.GetLowerSplit()[1];
                string channel = chatMessage.GetLowerSplit()[2];
                Message message = DataBase.GetFirstMessageUserChannel(username, channel);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (MessageNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendFuck(ChatMessage chatMessage)
        {
            string message = $"{Emoji.PointRight} {Emoji.OkHand} {chatMessage.Username} fucked {chatMessage.GetSplit()[1]}";
            if (chatMessage.GetSplit().Length > 2)
            {
                message += $" {chatMessage.GetSplit()[2]}";
            }
            return message;
        }

        public static void SendGoingAfk(this TwitchBot twitchBot, ChatMessage chatMessage, AfkCommandType type)
        {
            DataBase.SetAfk(chatMessage, type);
            twitchBot.Send(chatMessage.Channel, AfkMessage.Create(DataBase.GetUser(chatMessage.Username)).GoingAway);
        }

        public static string SendHelp(ChatMessage chatMessage)
        {
            string username = chatMessage.GetSplit().Length > 1 ? chatMessage.GetLowerSplit()[1] : chatMessage.Username;
            return $"{Emoji.PointRight} {username}, here you can find a list of commands and the repository: {Resources.GitHubRepoLink}";
        }

        public static string SendJoinChannel(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (Config.Moderators.Contains(chatMessage.Username))
            {
                string channel = chatMessage.GetLowerSplit()[1];
                twitchBot.JoinChannel(channel.RemoveHashtag(), out string response);
                return $"{chatMessage.Username}, {response}";
            }
            else
            {
                return $"{chatMessage.Username}, you are not a moderator of the bot";
            }
        }

        public static string SendLastMessage(ChatMessage chatMessage)
        {
            try
            {
                Message message = DataBase.GetLastMessage(chatMessage.GetLowerSplit()[1]);
                return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago")}) {message.Username}: {message.MessageText.Decode()}";
            }
            catch (UserNotFoundException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendLoggedMessagesChannelCount(ChatMessage chatMessage)
        {
            string channel = chatMessage.GetLowerSplit()[1];
            return $"{chatMessage.Username}, logging {new DottedNumber(new OkayegTeaTimeContext().CountChannelMessages(channel))} messages of the channel {channel}";
        }

        public static string SendLoggedMessagesCount(ChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, logging {new DottedNumber(new OkayegTeaTimeContext().CountMessages())} messages across all channels";
        }

        public static string SendLoggedMessagesUserCount(ChatMessage chatMessage)
        {
            string username = chatMessage.GetLowerSplit()[1];
            return $"{chatMessage.Username}, logging {new DottedNumber(new OkayegTeaTimeContext().CountUserMessages(username))} messages of {username}";
        }

        public static string SendMassping(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                string emote = chatMessage.GetSplit().Length > 1 ? chatMessage.GetSplit()[1] : EmoteInFrontHelper.GetEmote(chatMessage.Channel);
                string message = string.Empty;
                List<string> chatters;
                List<string> chattersToRemove = new() { chatMessage.Username };
                chattersToRemove = chattersToRemove.Concat(new JsonController().BotData.UserLists.SpecialUsers).ToList();

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

        public static string SendMathResult(ChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {HttpRequest.GetMathResult(chatMessage.GetMessage()[(chatMessage.GetSplit()[0].Length + 1)..])}";
        }

        public static string SendPick(ChatMessage chatMessage)
        {
            return $"{chatMessage.Username}, {chatMessage.GetSplit()[1..].Random()}";
        }

        public static string SendPing(TwitchBot twitchBot)
        {
            return $"Pongeg, I'm here! {twitchBot.GetSystemInfo()}";
        }

        public static string SendRandomCookie(ChatMessage chatMessage)
        {
            Pechkekse keks = DataBase.GetRandomCookie();
            return $"{chatMessage.Username}, {keks.Message} {Emoji.Cookie}";
        }

        public static string SendRandomGachi()
        {
            Gachi gachi = DataBase.GetRandomGachi();
            return $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS";
        }

        public static string SendRandomMessage(ChatMessage chatMessage)
        {
            try
            {
                if (chatMessage.GetSplit().Length > 2)
                {
                    Message message = DataBase.GetRandomMessage(chatMessage.GetLowerSplit()[1], chatMessage.GetLowerSplit()[2].RemoveHashtag());
                    return $"({message.Channel} | {TimeHelper.ConvertUnixTimeToTimeStamp(message.Time, "ago", ConversionType.YearDayHour)}) {message.Username}: {message.MessageText.Decode()}";
                }
                else if (chatMessage.GetSplit().Length > 1)
                {
                    Message message = DataBase.GetRandomMessage(chatMessage.GetLowerSplit()[1]);
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

        public static string SendRandomYourmom(ChatMessage chatMessage)
        {
            Yourmom yourmom = DataBase.GetRandomYourmom();
            string target = chatMessage.GetLowerSplit().Length > 1 ? chatMessage.GetLowerSplit()[1] : chatMessage.Username;
            return $"{target}, {yourmom.MessageText} YOURMOM";
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

        public static string SendResumingAfkStatus(ChatMessage chatMessage)
        {
            DataBase.ResumeAfkStatus(chatMessage.Username);
            User user = DataBase.GetUser(chatMessage.Username);
            return AfkMessage.Create(user).Resuming;
        }

        public static string SendSearch(ChatMessage chatMessage, string keyword)
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

        public static string SendSearchChannel(ChatMessage chatMessage, string keyword, string channel)
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

        public static string SendSearchUser(ChatMessage chatMessage, string keyword, string username)
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

        public static string SendSearchUserChannel(ChatMessage chatMessage, string keyword, string username, string channel)
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

        public static string SendSetEmoteInFront(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                string emote = chatMessage.GetSplit()[2][..(chatMessage.GetSplit()[2].Length > Config.MaxEmoteInFrontLength ? Config.MaxEmoteInFrontLength : chatMessage.GetSplit()[2].Length)];
                DataBase.SetEmoteInFront(chatMessage.Channel, emote);
                EmoteInFrontHelper.Update(chatMessage.Channel, emote);
                return $"{chatMessage.Username}, emote set to: {emote}";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendSetPrefix(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                string prefix = chatMessage.GetLowerSplit()[2][..(chatMessage.GetLowerSplit()[2].Length > Config.MaxPrefixLength ? Config.MaxPrefixLength : chatMessage.GetLowerSplit()[2].Length)];
                DataBase.SetPrefix(chatMessage.Channel, prefix);
                return $"{chatMessage.Username}, prefix set to: {prefix}";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendSetReminder(ChatMessage chatMessage, byte[] message)
        {
            try
            {
                string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}"));
                return $"{chatMessage.Username}, set a reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})";
            }
            catch (TooManyReminderException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSetSongRequestState(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                OkayegTeaTimeContext database = new();
                bool state = chatMessage.GetSplit()[2].IsMatch(@"(1|true|enabled?)");
                if (database.Spotify.Any(s => s.Username == chatMessage.Channel))
                {
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

        public static string SendSetTimedReminder(ChatMessage chatMessage, byte[] message, long toTime)
        {
            try
            {
                string target = chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1];
                int id = DataBase.AddReminder(new(chatMessage.Username, target, message, $"#{chatMessage.Channel}", toTime + TimeHelper.Now()));
                return $"{chatMessage.Username}, set a timed reminder for {GetReminderTarget(target, chatMessage.Username)} (ID: {id})";
            }
            catch (TooManyReminderException ex)
            {
                return $"{chatMessage.Username}, {ex.Message}";
            }
        }

        public static string SendSongAddedToQueue(ChatMessage chatMessage)
        {
            string song = chatMessage.GetSplit()[1];
            return $"{chatMessage.Username}, {SpotifyRequest.AddToQueue(chatMessage.Channel, song).Result}";
        }

        public static string SendSongSkipped(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                return $"{chatMessage.Username}, {SpotifyRequest.SkipToNextSong(chatMessage.Channel).Result}";
            }
            else
            {
                return $"{chatMessage.Username}, you have to be a mod or the broadcaster to skip the song";
            }
        }

        public static string SendSpotifyCurrentlyPlaying(ChatMessage chatMessage)
        {
            string username = chatMessage.GetLowerSplit().Length > 1 ? (chatMessage.GetLowerSplit()[1] == "me" ? chatMessage.Username : chatMessage.GetLowerSplit()[1]) : chatMessage.Channel;
            return $"{chatMessage.Username}, {SpotifyRequest.GetCurrentlyPlaying(username).Result}";
        }

        public static string SendSpotifySearch(ChatMessage chatMessage)
        {
            string query = chatMessage.GetSplit().Skip(2).ToSequence();
            return $"{chatMessage.Username}, {SpotifyRequest.Search(query).Result}";
        }

        public static string SendSuggestionNoted(ChatMessage chatMessage)
        {
            DataBase.AddSugestion(chatMessage, chatMessage.GetMessage()[chatMessage.GetLowerSplit()[0].Length..]);
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

        public static string SendTuckedToBed(ChatMessage chatMessage)
        {
            string target = chatMessage.GetLowerSplit()[1];
            string emote = chatMessage.GetLowerSplit().Length > 2 ? chatMessage.GetSplit()[2] : string.Empty;
            return $"{Emoji.PointRight} {Emoji.Bed} {chatMessage.Username} tucked {target} to bed {emote}".Trim();
        }

        public static string SendUnsetEmoteInFront(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.UnsetEmoteInFront(chatMessage.Channel);
                EmoteInFrontHelper.Update(chatMessage.Channel, null);
                return $"{chatMessage.Username}, unset emote";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendUnsetNuke(ChatMessage chatMessage)
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

        public static string SendUnsetPrefix(ChatMessage chatMessage)
        {
            if (chatMessage.IsModOrBroadcaster())
            {
                DataBase.UnsetPrefix(chatMessage.Channel);
                return $"{chatMessage.Username}, the prefix has been unset";
            }
            else
            {
                return $"{chatMessage.Username}, you aren't a mod or the broadcaster";
            }
        }

        public static string SendUnsetReminder(ChatMessage chatMessage)
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

        public static string SendUserID(ChatMessage chatMessage)
        {
            try
            {
                string username = chatMessage.GetSplit().Length > 1 ? chatMessage.GetLowerSplit()[1] : chatMessage.Username;
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
    }
}
