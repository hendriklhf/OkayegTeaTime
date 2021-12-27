using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using Microsoft.EntityFrameworkCore;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Exceptions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Database;

public static class DatabaseController
{
    private const string _noModOrStreamerMessage = "you aren't a mod or the broadcaster";
    private const string _noPermissionToDeleteReminderMessage = "you have no permission to delete the reminder of someone else";

    public static void AddChannel(string channel)
    {
        // FIXME: all operations create a Context, act on it and dispose straight away
        // Would be better to make this class non-static (treat as a Repository) & pool connections (see the following:
        // https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#dbcontext-pooling)
        using var database = new OkayegTeaTimeContext();
        database.Channels.Add(new(channel));
        database.SaveChanges();
    }

    public static void AddMessage(ITwitchChatMessage chatMessage)
    {
        if (!AppSettings.NotLoggedChannels.Contains(chatMessage.Channel.Name))
        {
            using var database = new OkayegTeaTimeContext();
            database.Messages.Add(new(chatMessage.Username, chatMessage.Message.Encode(), chatMessage.Channel.Name));
            database.SaveChanges();
        }
    }

    public static void AddNewToken(string username, string accessToken, string refreshToken)
    {
        using var database = new OkayegTeaTimeContext();
        if (database.Spotify.Any(s => s.Username == username))
        {
            Models.Spotify user = database.Spotify.FirstOrDefault(s => s.Username == username);
            user.AccessToken = accessToken;
            user.RefreshToken = refreshToken;
            user.Time = TimeHelper.Now();
            database.SaveChanges();
        }
        else
        {
            database.Spotify.Add(new(username, accessToken, refreshToken));
            database.SaveChanges();
        }
    }

    public static int AddNuke(Nuke nuke)
    {
        using var database = new OkayegTeaTimeContext();
        database.Nukes.Add(nuke);
        database.SaveChanges();
        return database.Nukes.FirstOrDefault(n =>
            n.Channel == nuke.Channel
            && n.ForTime == nuke.ForTime
            && n.TimeoutTime == nuke.TimeoutTime
            && n.Username == nuke.Username
            && n.Word == nuke.Word).Id;
    }

    public static int AddReminder(Reminder reminder)
    {
        using var database = new OkayegTeaTimeContext();
        if (reminder.ToTime == 0)
        {
            if (database.Reminders.AsQueryable().Where(r => r.ToUser == reminder.ToUser && r.ToTime == 0).Count() >= AppSettings.MaxReminders)
            {
                throw new TooManyReminderException();
            }
        }
        else
        {
            if (database.Reminders.AsQueryable().Where(r => r.ToUser == reminder.ToUser && r.ToTime != 0).Count() >= AppSettings.MaxReminders)
            {
                throw new TooManyReminderException();
            }
        }
        database.Reminders.Add(reminder);
        database.SaveChanges();
        return database.Reminders.FirstOrDefault(r => r.FromUser == reminder.FromUser
            && r.ToUser == reminder.ToUser
            && r.Message == reminder.Message
            && r.ToTime == reminder.ToTime
            && r.Time == reminder.Time).Id;
    }

    public static void AddSugestion(ITwitchChatMessage chatMessage, string suggestion)
    {
        using var database = new OkayegTeaTimeContext();
        database.Suggestions.Add(new(chatMessage.Username, suggestion.Encode(), $"#{chatMessage.Channel}"));
        database.SaveChanges();
    }

    public static void AddUser(string username)
    {
        using var database = new OkayegTeaTimeContext();
        if (!database.Users.Any(u => u.Username == username))
        {
            database.Users.Add(new User(username));
            database.SaveChanges();
        }
    }

    public static void CheckForNukes(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        if (!chatMessage.IsAnyCommand)
        {
            using var database = new OkayegTeaTimeContext();
            var prefixedChannel = $"#{chatMessage.Channel}";

            if (database.Nukes.Any(n => n.Channel == prefixedChannel))
            {
                database.Nukes.AsQueryable().Where(n => n.Channel == prefixedChannel).ForEach(n =>
                {
                    if (n.ForTime > TimeHelper.Now())
                    {
                        if (!chatMessage.IsModerator || !chatMessage.IsBroadcaster)
                        {
                            if (chatMessage.Message.IsMatch(n.Word.Decode()))
                            {
                                twitchBot.Timeout(chatMessage.Channel.Name, chatMessage.Username, n.TimeoutTime, Nuke.Reason);
                            }
                        }
                    }
                    else
                    {
                        database.Nukes.Remove(n);
                        database.SaveChanges();
                    }
                });
            }
        }
    }

    public static void CheckForReminder(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        if (database.Reminders.Any(reminder => reminder.ToTime == 0 && reminder.ToUser == chatMessage.Username))
        {
            List<Reminder> listReminder = database.Reminders.AsQueryable().Where(reminder => reminder.ToTime == 0 && reminder.ToUser == chatMessage.Username).ToList();
            twitchBot.SendReminder(chatMessage, listReminder);
            RemoveReminder(listReminder);
        }
    }

    public static void CheckForTimedReminder(TwitchBot twitchBot)
    {
        using var database = new OkayegTeaTimeContext();
        if (database.Reminders.Any(reminder => reminder.ToTime != 0))
        {
            List<Reminder> listReminder = database.Reminders.AsQueryable().Where(reminder => reminder.ToTime != 0 && reminder.ToTime <= TimeHelper.Now()).ToList();
            listReminder.ForEach(reminder =>
            {
                twitchBot.SendTimedReminder(reminder);
                database.Reminders.Remove(reminder);
            });
            database.SaveChanges();
        }
    }

    public static void CheckIfAFK(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        User user = database.Users.FirstOrDefault(user => user.Username == chatMessage.Username);
        if (user.IsAfk == true)
        {
            twitchBot.SendComingBack(user, chatMessage);
            if (!chatMessage.IsAfkCommmand)
            {
                SetAfk(chatMessage.Username, false);
            }
        }
    }

    public static int CountChannelMessages(string givenChannel)
    {
        return new OkayegTeaTimeContext().Messages.AsQueryable().Where(m => m.Channel == givenChannel).Count();
    }

    public static int CountMessages()
    {
        return new OkayegTeaTimeContext().Messages.Count();
    }

    public static int CountUserMessages(string givenUsername)
    {
        return new OkayegTeaTimeContext().Messages.AsQueryable().Where(m => m.Username == givenUsername).Count();
    }

    public static bool DoesSpotifyUserExist(string username)
    {
        return new OkayegTeaTimeContext().Spotify.Any(s => s.Username == username.ToLower());
    }

    public static List<string> GetChannels()
    {
        return new OkayegTeaTimeContext().Channels.AsQueryable().Select(c => c.ChannelName).ToList();
    }

    public static string GetEmoteInFront(string channel)
    {
        return new OkayegTeaTimeContext().Channels.FirstOrDefault(c => c.ChannelName == channel).EmoteInFront?.Decode();
    }

    public static List<string> GetEmoteManagementSubs()
    {
        return new OkayegTeaTimeContext().Channels.AsQueryable().Where(c => c.EmoteManagementSub == true).Select(c => c.ChannelName).ToList();
    }

    public static Dictionary<string, string> GetEmotesInFront()
    {
        return new OkayegTeaTimeContext().Channels.ToDictionary(c => c.ChannelName, c => c.EmoteInFront?.Decode());
    }

    public static Message GetFirst(ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FirstOrDefault(m => m.Username == chatMessage.Username);
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetFirstChannel(ITwitchChatMessage chatMessage, string channel)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FirstOrDefault(m => m.Username == chatMessage.Username && m.Channel == $"#{channel.RemoveHashtag()}");
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetFirstMessageUserChannel(string username, string channel)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FirstOrDefault(m => m.Username == username && channel == $"#{channel.RemoveHashtag()}");
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetFirstUser(string username)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FirstOrDefault(m => m.Username == username);
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetLastMessage(string username)
    {

        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.AsQueryable().Where(m => m.Username == username).OrderByDescending(m => m.Id).FirstOrDefault();
        return message ?? throw new UserNotFoundException();
    }

    public static Message GetMessage(int id)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FirstOrDefault(m => m.Id == id);
        return message ?? throw new MessageNotFoundException();
    }

    public static string GetPrefix(string channel)
    {
        return new OkayegTeaTimeContext().Channels.FirstOrDefault(c => c.ChannelName == channel).Prefix?.Decode();
    }

    public static Dictionary<string, string> GetPrefixes()
    {
        return new OkayegTeaTimeContext().Channels.ToDictionary(c => c.ChannelName, c => c.Prefix?.Decode());
    }

    public static Pechkekse GetRandomCookie()
    {
        using var database = new OkayegTeaTimeContext();
        return database.Pechkekse.FromSqlRaw($"SELECT * FROM pechkekse ORDER BY RAND() LIMIT 1").FirstOrDefault();
    }

    public static Gachi GetRandomGachi()
    {
        using var database = new OkayegTeaTimeContext();
        return database.Gachi.FromSqlRaw($"SELECT * FROM gachi ORDER BY RAND() LIMIT 1").FirstOrDefault();
    }

    public static Message GetRandomMessage(ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE channel = '#{chatMessage.Channel}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetRandomMessage(string username)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE username = '{username.RemoveSQLChars()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetRandomMessage(string username, string channel)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE username ='{username.RemoveSQLChars()}' AND channel = '#{channel.RemoveSQLChars()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Yourmom GetRandomYourmom()
    {
        using var database = new OkayegTeaTimeContext();
        return database.Yourmom.FromSqlRaw($"SELECT * FROM yourmom ORDER BY RAND() LIMIT 1").FirstOrDefault();
    }

    public static string GetRefreshToken(string username)
    {
        return new OkayegTeaTimeContext().Spotify.FirstOrDefault(s => s.Username == username).RefreshToken;
    }

    public static Reminder GetReminder(int id)
    {
        Reminder reminder = new OkayegTeaTimeContext().Reminders.FirstOrDefault(r => r.Id == id);
        return reminder ?? throw new ReminderNotFoundException();
    }

    public static Message GetSearch(string keyword)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.RemoveSQLChars()}%' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetSearchChannel(string keyword, string channel)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.RemoveSQLChars()}%' AND Channel = '#{channel.RemoveHashtag().RemoveSQLChars().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetSearchUser(string keyword, string username)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.RemoveSQLChars()}%' AND Username = '{username.RemoveSQLChars().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Message GetSearchUserChannel(string keyword, string username, string channel)
    {
        using var database = new OkayegTeaTimeContext();
        Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.RemoveSQLChars()}%' AND Username = '{username.RemoveSQLChars().ToLower()}' AND Channel = '#{channel.RemoveHashtag().RemoveSQLChars().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        return message ?? throw new MessageNotFoundException();
    }

    public static Models.Spotify GetSpotifyUser(string username)
    {
        return new OkayegTeaTimeContext().Spotify.FirstOrDefault(s => s.Username == username);
    }

    public static User GetUser(string username)
    {
        using var database = new OkayegTeaTimeContext();
        User user = database.Users.FirstOrDefault(u => u.Username == username);
        return user ?? throw new UserNotFoundException();
    }

    public static bool IsEmoteManagementSub(string channel)
    {
        return new OkayegTeaTimeContext().Channels.FirstOrDefault(c => c.ChannelName == channel.ToLower()).EmoteManagementSub == true;
    }

    public static void RemoveNuke(ITwitchChatMessage chatMessage)
    {
        int id = chatMessage.Split[2].ToInt();
        using var database = new OkayegTeaTimeContext();
        Nuke nuke = database.Nukes.FirstOrDefault(n => n.Id == id && n.Channel == $"#{chatMessage.Channel.Name.RemoveHashtag()}");
        if (nuke is not null)
        {
            if (chatMessage.IsBroadcaster || chatMessage.IsModerator || AppSettings.UserLists.Moderators.Contains(chatMessage.Username))
            {
                database.Nukes.Remove(nuke);
                database.SaveChanges();
            }
            else
            {
                throw new NoPermissionException(_noModOrStreamerMessage);
            }
        }
        else
        {
            throw new NukeNotFoundException();
        }
    }

    public static void RemoveReminder(ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        Reminder reminder = database.Reminders.FirstOrDefault(r => r.Id == chatMessage.Split[2].ToInt());
        if (reminder is not null)
        {
            if (reminder.FromUser == chatMessage.Username
                || (reminder.ToUser == chatMessage.Username && reminder.ToTime != 0)
                || AppSettings.UserLists.Moderators.Contains(chatMessage.Username))
            {
                database.Reminders.Remove(reminder);
                database.SaveChanges();
            }
            else
            {
                throw new NoPermissionException(_noPermissionToDeleteReminderMessage);
            }
        }
        else
        {
            throw new ReminderNotFoundException();
        }
    }

    public static void ResumeAfkStatus(string username)
    {
        SetAfk(username, true);
    }

    public static void SetAfk(ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        using var database = new OkayegTeaTimeContext();
        User user = database.Users.FirstOrDefault(u => u.Username == chatMessage.Username);
        string message = chatMessage.Split.Length > 1 ? chatMessage.Split[1..].ToSequence() : null;
        user.MessageText = message?.Encode();
        user.Type = type.ToString();
        user.Time = TimeHelper.Now();
        database.SaveChanges();
        SetAfk(chatMessage.Username, true);
    }

    public static void SetEmoteInFront(string channel, string emote)
    {
        using var database = new OkayegTeaTimeContext();
        database.Channels.FirstOrDefault(c => c.ChannelName == channel).EmoteInFront = emote.Encode();
        database.SaveChanges();
    }

    public static void SetEmoteSub(string channel, bool subbed)
    {
        using var database = new OkayegTeaTimeContext();
        database.Channels.FirstOrDefault(c => c.ChannelName == channel).EmoteManagementSub = subbed;
        database.SaveChanges();
    }

    public static void SetPrefix(string channel, string prefix)
    {
        using var database = new OkayegTeaTimeContext();
        database.Channels.FirstOrDefault(c => c.ChannelName == channel).Prefix = prefix.RemoveChatterinoChar().TrimAll().Encode();
        database.SaveChanges();
    }

    public static void SetSongRequestEnabledState(string channel, bool enabled)
    {
        using var database = new OkayegTeaTimeContext();
        Models.Spotify user = database.Spotify.FirstOrDefault(s => s.Username == channel.ToLower());
        if (user is not null)
        {
            user.SongRequestEnabled = enabled;
            database.SaveChanges();
        }
    }

    public static void UnsetEmoteInFront(string channel)
    {
        using var database = new OkayegTeaTimeContext();
        database.Channels.FirstOrDefault(c => c.ChannelName == channel).EmoteInFront = null;
        database.SaveChanges();
    }

    public static void UnsetPrefix(string channel)
    {
        using var database = new OkayegTeaTimeContext();
        database.Channels.FirstOrDefault(c => c.ChannelName == channel).Prefix = null;
        database.SaveChanges();
    }

    public static void UpdateAccessToken(string username, string accessToken)
    {
        using var database = new OkayegTeaTimeContext();
        Models.Spotify user = database.Spotify.FirstOrDefault(s => s.Username == username);
        user.AccessToken = accessToken;
        user.Time = TimeHelper.Now();
        database.SaveChanges();
    }

    private static void RemoveReminder(List<Reminder> listReminder)
    {
        using var database = new OkayegTeaTimeContext();
        listReminder.ForEach(reminder => database.Reminders.Remove(reminder));
        database.SaveChanges();
    }

    private static void SetAfk(string username, bool afk)
    {
        using var database = new OkayegTeaTimeContext();
        database.Users.AsQueryable().Where(u => u.Username == username).FirstOrDefault().IsAfk = afk;
        database.SaveChanges();
    }
}
