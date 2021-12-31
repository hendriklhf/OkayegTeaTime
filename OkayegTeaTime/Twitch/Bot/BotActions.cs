using System.Text;
using HLE.Collections;
using HLE.Emojis;
using HLE.HttpRequests;
using HLE.Numbers;
using HLE.Strings;
using HLE.Time;
using HLE.Time.Enums;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Exceptions;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;
using Random = HLE.Random.Random;

namespace OkayegTeaTime.Twitch.Bot;

public static class BotActions
{
    private const string _channelEmotesError = "the channel doesn't have the specified amount of " +
        "emotes enabled or an error occurred";
    private const byte _defaultEmoteCount = 5;
    private const string _noModOrStreamerMessage = "you aren't a mod or the broadcaster";
    private const string _twitchUserDoesntExistMessage = "Twitch user doesn't exist";
    private const string _tooManyRemindersMessage = "that person has too many reminders set for them";

    public static void AddAfkCooldown(int userId)
    {
        if (TwitchBot.AfkCooldowns.Any(c => c.UserId == userId))
        {
            TwitchBot.AfkCooldowns.Remove(TwitchBot.AfkCooldowns.FirstOrDefault(c => c.UserId == userId));
            AddUserToAfkCooldownDictionary(userId);
        }
    }

    public static string SendSongAddedToQueue(ITwitchChatMessage chatMessage)
    {
        string song = chatMessage.Split[1];
        return $"{chatMessage.Username}, {SpotifyRequest.AddToQueue(chatMessage.Channel.Name, song).Result}";
    }

    public static void AddCooldown(int userId, CommandType type)
    {
        if (TwitchBot.Cooldowns.Any(c => c.UserId == userId && c.Type == type))
        {
            TwitchBot.Cooldowns.Remove(TwitchBot.Cooldowns.FirstOrDefault(c => c.UserId == userId && c.Type == type));
            AddUserToCooldownDictionary(userId, type);
        }
    }

    public static void AddUserToAfkCooldownDictionary(int userId)
    {
        if (!AppSettings.UserLists.Moderators.Contains(userId))
        {
            if (!TwitchBot.AfkCooldowns.Any(c => c.UserId == userId))
            {
                TwitchBot.AfkCooldowns.Add(new(userId));
            }
        }
    }

    public static void AddUserToCooldownDictionary(int userId, CommandType type)
    {
        if (!AppSettings.UserLists.Moderators.Contains(userId))
        {
            if (!TwitchBot.Cooldowns.Any(c => c.UserId == userId && c.Type == type))
            {
                TwitchBot.Cooldowns.Add(new(userId, type));
            }
        }
    }

    public static bool IsOnAfkCooldown(int userId)
    {
        return TwitchBot.AfkCooldowns.Any(c => c.UserId == userId && c.Time > TimeHelper.Now());
    }

    public static bool IsOnCooldown(int userId, CommandType type)
    {
        return TwitchBot.Cooldowns.Any(c => c.UserId == userId && c.Type == type && c.Time > TimeHelper.Now());
    }

    public static void SendBanFromFile(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        try
        {
            if (AppSettings.UserLists.Moderators.Contains(chatMessage.UserId))
            {
                List<string> fileContent = new HttpGet(chatMessage.Split[1]).Result.Split("\n").ToList();
                string regex = chatMessage.Split[2];
                fileContent.Where(f => f.IsMatch(regex)).ForEach(f =>
                {
                    if (f.IsMatch(@"^[\./]ban\s\w+"))
                    {
                        twitchBot.TwitchClient.SendMessage(chatMessage.Channel.Name, f);
                    }
                    else
                    {
                        twitchBot.TwitchClient.SendMessage(chatMessage.Channel.Name, $"/ban {f}");
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
        string channel = chatMessage.LowerSplit.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Channel.Name;
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
            User user = DatabaseController.GetUser(username);
            if (user.IsAfk == true)
            {
                string message = $"{chatMessage.Username}, {new AfkMessage(user).GoingAway}";
                message += user.MessageText.Decode().Length > 0
                    ? $": {user.MessageText.Decode()} ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})"
                    : $" ({TimeHelper.ConvertUnixTimeToTimeStamp(user.Time, "ago", ConversionType.YearDayHourMin)})";
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

    public static string SendCheckReminder(IChatMessage chatMessage)
    {
        try
        {
            int id = chatMessage.Split[2].ToInt();
            Reminder reminder = DatabaseController.GetReminder(id);
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
        string result = Random.Int(0, 100) >= 50 ? "yes/heads" : "no/tails";
        return $"{chatMessage.Username}, {result} {Emoji.Coin}";
    }

    public static void SendComingBack(this TwitchBot twitchBot, User user, ITwitchChatMessage chatMessage)
    {
        twitchBot.Send(chatMessage.Channel, new AfkMessage(user).ComingBack);
    }

    public static string SendCompilerResult(IChatMessage chatMessage)
    {
        return $"{chatMessage.Username}, {HttpRequest.GetCSharpOnlineCompilerResult(chatMessage.Message[(chatMessage.Split[0].Length + 1)..])}";
    }

    public static string SendDetectedSpotifyUri(IChatMessage chatMessage)
    {
        if (new LinkRecognizer(chatMessage).TryFindSpotifyLink(out string uri))
        {
            return $"{uri}";
        }
        else
        {
            return null;
        }
    }

    public static string SendFill(ITwitchChatMessage chatMessage)
    {
        List<string> messageParts = new();
        string[] split = chatMessage.Split[1..];
        int maxLength = AppSettings.MaxMessageLength - (chatMessage.Channel.Emote.Length + 1);
        for (; ; )
        {
            string messagePart = split.Random();
            int currentMessageLength = messageParts.Sum(m => m.Length) + messageParts.Count + messagePart.Length;
            if (currentMessageLength <= maxLength)
            {
                messageParts.Add(messagePart);
            }
            else
            {
                break;
            }
        }
        return string.Join((char)32, messageParts);
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
        DatabaseController.SetAfk(chatMessage, type);
        twitchBot.Send(chatMessage.Channel, new AfkMessage(DatabaseController.GetUser(chatMessage.Username)).GoingAway);
    }

    public static string SendGoLangCompilerResult(IChatMessage chatMessage)
    {
        return $"{chatMessage.Username}, {HttpRequest.GetGoLangOnlineCompilerResult(chatMessage.Message[(chatMessage.Message.Split()[0].Length + 1)..])}";
    }

    public static string SendHelp(IChatMessage chatMessage)
    {
        string username = chatMessage.Split.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
        return $"{Emoji.PointRight} {username}, here you can find a list of commands and the repository: {AppSettings.RepositoryUrl}";
    }

    public static string SendJoinChannel(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        if (AppSettings.UserLists.Moderators.Contains(chatMessage.UserId))
        {
            string channel = chatMessage.LowerSplit[1];
            string response = twitchBot.JoinChannel(channel.Remove("#"));
            return $"{chatMessage.Username}, {response}";
        }
        else
        {
            return $"{chatMessage.Username}, you are not a moderator of the bot";
        }
    }

    public static string SendMassping(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster && chatMessage.Channel.Name != "moondye7")
        {
            string emote = chatMessage.Split.Length > 1 ? chatMessage.Split[1] : chatMessage.Channel.Emote;
            string message = string.Empty;
            List<string> chatters;
            if (chatMessage.Channel.Name != AppSettings.SecretOfflineChatChannel)
            {
                chatters = HttpRequest.GetChatters(chatMessage.Channel.Name).Select(c => c.Username).ToList();
                if (chatters.Count == 0)
                {
                    return string.Empty;
                }
            }
            else
            {
                message = $"OkayegTeaTime {emote} ";
                chatters = AppSettings.SecretOfflineChatEmotes;
            }
            message += string.Join($" {emote} ", chatters);
            return message;
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
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
        return $"Pongeg, I'm here! {twitchBot.SystemInfo}";
    }

    public static string SendRandomGachi()
    {
        Gachi gachi = DatabaseController.GetRandomGachi();
        return $"{Emoji.PointRight} {gachi.Title.Decode()} || {gachi.Link} gachiBASS";
    }

    public static string SendRandomWords(ITwitchChatMessage chatMessage, int count = 1)
    {
        List<string> words = new();
        for (int i = 0; i < count; i++)
        {
            words.Add(AppSettings.RandomWords.Random());
        }
        string message = $"{chatMessage.Username}, {words.JoinToString(' ')}";
        if (MessageHelper.IsMessageTooLong(message, chatMessage.Channel))
        {
            return $"{message[..(AppSettings.MaxMessageLength - (3 + chatMessage.Channel.Emote.Length + 1))]}...";
        }
        else
        {
            return message;
        }
    }

    public static string SendRandomYourmom(IChatMessage chatMessage)
    {
        Yourmom yourmom = DatabaseController.GetRandomYourmom();
        string target = chatMessage.LowerSplit.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
        return $"{target}, {yourmom.MessageText} YOURMOM";
    }

    public static void SendReminder(this TwitchBot twitchBot, ITwitchChatMessage chatMessage, List<Reminder> reminders)
    {
        if (!reminders.Any())
        {
            return;
        }

        string message = $"{chatMessage.Username}, reminder from {GetReminderAuthor(chatMessage.Username, reminders[0].FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminders[0].Time, "ago")})";
        StringBuilder builder = new(message);
        if (reminders[0].Message.Length > 0)
        {
            builder.Append($": {reminders[0].Message.Decode()}");
        }

        if (reminders.Count > 1)
        {
            reminders.Skip(1).ForEach(r =>
            {
                builder.Append($" || {GetReminderAuthor(chatMessage.Username, r.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(r.Time, "ago")})");
                if (r.Message.Length > 0)
                {
                    builder.Append($": {r.Message.Decode()}");
                }
            });
        }
        twitchBot.Send(chatMessage.Channel, builder.ToString());
    }

    public static string SendResumingAfkStatus(IChatMessage chatMessage)
    {
        DatabaseController.ResumeAfkStatus(chatMessage.Username);
        User user = DatabaseController.GetUser(chatMessage.Username);
        return new AfkMessage(user).Resuming;
    }

    public static string SendSetEmoteInFront(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
        {
            string emote = chatMessage.Split[2][..(chatMessage.Split[2].Length > AppSettings.MaxEmoteInFrontLength ? AppSettings.MaxEmoteInFrontLength : chatMessage.Split[2].Length)];
            chatMessage.Channel.Emote = emote;
            return $"{chatMessage.Username}, emote set to: {emote}";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendSetPrefix(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
        {
            string prefix = chatMessage.LowerSplit[2][..(chatMessage.LowerSplit[2].Length > AppSettings.MaxPrefixLength ? AppSettings.MaxPrefixLength : chatMessage.LowerSplit[2].Length)];
            chatMessage.Channel.Prefix = prefix;
            return $"{chatMessage.Username}, prefix set to: {prefix}";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendSetReminder(ITwitchChatMessage chatMessage, string[] targets, string message, long toTime = 0)
    {
        if (targets.Length == 1)
        {
            bool targetExists = TwitchApi.DoesUserExist(targets[0]);
            if (!targetExists)
            {
                return $"{chatMessage.Username}, the target user does not exist";
            }

            int? id = DatabaseController.AddReminder(chatMessage.Username, targets[0], message, chatMessage.Channel.Name, toTime);
            if (!id.HasValue)
            {
                return $"{chatMessage.Username}, {_tooManyRemindersMessage}";
            }

            return $"{chatMessage.Username}, set a {(toTime == 0 ? string.Empty : "timed ")}reminder for {GetReminderTarget(targets[0], chatMessage.Username)} (ID: {id.Value})";
        }
        else
        {
            Dictionary<string, bool> exist = TwitchApi.DoUsersExist(targets);

            (string, string, string, string, long)[] values = targets
                .Where(t => exist[t])
                .Select<string, (string, string, string, string, long)>(t => new(chatMessage.Username, t, message, chatMessage.Channel.Name, toTime))
                .ToArray();

            int?[] ids = DatabaseController.AddReminders(values);

            StringBuilder builder = new($"{chatMessage.Username}, ");
            bool multi = ids.Count(i => i != default) > 1;
            if (!multi)
            {
                builder.Append(_tooManyRemindersMessage);
                return builder.ToString();
            }

            builder.Append($"set{(multi ? string.Empty : " a")} {(toTime == 0 ? string.Empty : "timed ")}reminder{(multi ? 's' : string.Empty)} for ");
            List<string> responses = new();
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] != default && exist[targets[i]])
                {
                    responses.Add($"{targets[i]} ({ids[i]})");
                }
            }

            builder.Append(string.Join(", ", responses));
            return builder.ToString();
        }
    }

    public static string SendSetSongRequestState(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
        {
            if (DatabaseController.DoesSpotifyUserExist(chatMessage.Channel.Name))
            {
                bool state = chatMessage.Split[2].IsMatch(@"(1|true|enabled?)");
                DatabaseController.SetSongRequestEnabledState(chatMessage.Channel.Name, state);
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

    public static string SendSongSkipped(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
        {
            return $"{chatMessage.Username}, {SpotifyRequest.SkipToNextSong(chatMessage.Channel.Name).Result}";
        }
        else
        {
            return $"{chatMessage.Username}, you have to be a mod or the broadcaster to skip the song";
        }
    }

    public static string SendSpotifyCurrentlyPlaying(ITwitchChatMessage chatMessage)
    {
        string username = chatMessage.LowerSplit.Length > 1 ? (chatMessage.LowerSplit[1] == "me" ? chatMessage.Username : chatMessage.LowerSplit[1]) : chatMessage.Channel.Name;
        return $"{chatMessage.Username}, {SpotifyRequest.GetCurrentlyPlaying(username).Result}";
    }

    public static string SendSpotifySearch(IChatMessage chatMessage)
    {
        string query = chatMessage.Split.Skip(2).JoinToString(' ');
        return $"{chatMessage.Username}, {SpotifyRequest.Search(query).Result}";
    }

    public static string SendSubbedToEmotes(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsBroadcaster || chatMessage.IsModerator)
        {
            twitchBot.EmoteManagementNotificator?.AddChannel(chatMessage.Channel.Name);
            chatMessage.Channel.IsEmoteSub = true;
            return $"{chatMessage.Username}, channel #{chatMessage.Channel} has subscribed to the emote notifications";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendSuggestionNoted(ITwitchChatMessage chatMessage)
    {
        DatabaseController.AddSugestion(chatMessage, chatMessage.Message[chatMessage.LowerSplit[0].Length..]);
        return $"{chatMessage.Username}, your suggestion has been noted";
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string message = $"{reminder.ToUser}, reminder from {GetReminderAuthor(reminder.ToUser, reminder.FromUser)} ({TimeHelper.ConvertUnixTimeToTimeStamp(reminder.Time, "ago")})";
        string reminderMessage = reminder.Message.Decode();
        if (!string.IsNullOrEmpty(reminderMessage))
        {
            message += $": {reminderMessage}";
        }
        twitchBot.Send(reminder.Channel, message);
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
            chatMessage.Channel.Emote = null;
            return $"{chatMessage.Username}, unset emote";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendUnsetPrefix(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsModerator || chatMessage.IsBroadcaster)
        {
            chatMessage.Channel.Prefix = null;
            return $"{chatMessage.Username}, the prefix has been unset";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendUnsetReminder(ITwitchChatMessage chatMessage)
    {
        try
        {
            DatabaseController.RemoveReminder(chatMessage);
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

    public static string SendUnsubEmotes(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        if (chatMessage.IsBroadcaster || chatMessage.IsModerator)
        {
            twitchBot.EmoteManagementNotificator?.RemoveChannel(chatMessage.Channel.Name);
            chatMessage.Channel.IsEmoteSub = false;
            return $"{chatMessage.Username}, channel #{chatMessage.Channel} has unsubscribed from the emote notifications";
        }
        else
        {
            return $"{chatMessage.Username}, {_noModOrStreamerMessage}";
        }
    }

    public static string SendUserId(IChatMessage chatMessage)
    {
        string username = chatMessage.Split.Length > 1 ? chatMessage.LowerSplit[1] : chatMessage.Username;
        return $"{chatMessage.Username}, {TwitchApi.GetUserId(username)?.ToString() ?? _twitchUserDoesntExistMessage}";
    }

    public static void Timeout(this TwitchBot twitchBot, string channel, string username, long time, string reason = "")
    {
        twitchBot.TwitchClient.SendMessage(channel, $"/timeout {username} {time} {reason}".Trim());
    }

    private static string GetReminderAuthor(string toUser, string fromUser)
    {
        return toUser == fromUser ? "yourself" : fromUser;
    }

    private static string GetReminderTarget(string toUser, string fromUser)
    {
        return toUser == fromUser ? "yourself" : toUser;
    }
}
