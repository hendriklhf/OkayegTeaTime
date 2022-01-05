using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Exceptions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Database;

public static class DatabaseController
{
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

    public static int? AddReminder(string fromUser, string toUser, string message, string channel, long toTime = 0)
    {
        using var database = new OkayegTeaTimeContext();
        Reminder reminder = new(fromUser, toUser, message.Encode(), channel, toTime);

        if (HasTooManyRemindersSet(reminder.ToUser, reminder.ToTime > 0))
        {
            return null;
        }

        EntityEntry<Reminder> entity = database.Reminders.Add(reminder);
        database.SaveChanges();
        return entity.Entity.Id;
    }

    public static int?[] AddReminders(IEnumerable<(string FromUser, string ToUser, string Message, string Channel, long ToTime)> reminders)
    {
        int count = reminders.Count();
        EntityEntry<Reminder>[] entities = new EntityEntry<Reminder>[count];
        using OkayegTeaTimeContext database = new();
        reminders.ForEach((v, i) =>
        {
            Reminder r = new(v);
            if (HasTooManyRemindersSet(r.ToUser, r.ToTime > 0, database))
            {
                entities[i] = null;
            }
            else
            {
                entities[i] = database.Reminders.Add(r);
            }
        });
        database.SaveChanges();
        return entities.Select(e => e?.Entity?.Id).ToArray();
    }

    public static int?[] AddReminders(IEnumerable<(string FromUser, string ToUser, string Message, string Channel)> reminders)
    {
        int count = reminders.Count();
        EntityEntry<Reminder>[] entities = new EntityEntry<Reminder>[count];
        using OkayegTeaTimeContext database = new();
        reminders.ForEach((v, i) =>
        {
            Reminder r = new(v);
            if (HasTooManyRemindersSet(r.ToUser, r.ToTime > 0, database))
            {
                entities[i] = null;
            }
            else
            {
                entities[i] = database.Reminders.Add(r);
            }
        });
        database.SaveChanges();
        return entities.Select(e => e?.Entity?.Id).ToArray();
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
            database.Users.Add(new(username));
            database.SaveChanges();
        }
    }

    public static void CheckForReminder(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        List<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime == 0 && r.ToUser == chatMessage.Username).ToList();
        twitchBot.SendReminder(chatMessage, reminders);
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    public static void CheckForTimedReminder(TwitchBot twitchBot)
    {
        using var database = new OkayegTeaTimeContext();
        IEnumerable<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime != 0 && r.ToTime <= TimeHelper.Now());
        reminders.ForEach(r => twitchBot.SendTimedReminder(r));
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    public static void CheckIfAfk(TwitchBot twitchBot, ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        User user = database.Users.FirstOrDefault(user => user.Username == chatMessage.Username);
        if (user.IsAfk == true)
        {
            twitchBot.SendComingBack(user, chatMessage);
            if (!chatMessage.IsAfkCommmand)
            {
                SetAfkStatus(chatMessage.Username, false);
            }
        }
    }

    public static bool DoesSpotifyUserExist(string username)
    {
        return new OkayegTeaTimeContext().Spotify.Any(s => s.Username == username.ToLower());
    }

    public static Channel GetChannel(string channel)
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.FirstOrDefault(c => c.ChannelName == channel);
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

    public static string GetPrefix(string channel)
    {
        return new OkayegTeaTimeContext().Channels.FirstOrDefault(c => c.ChannelName == channel).Prefix?.Decode();
    }

    public static Dictionary<string, string> GetPrefixes()
    {
        return new OkayegTeaTimeContext().Channels.ToDictionary(c => c.ChannelName, c => c.Prefix?.Decode());
    }

    public static Gachi GetRandomGachi()
    {
        using var database = new OkayegTeaTimeContext();
        return database.Gachi.FromSqlRaw($"SELECT * FROM gachi ORDER BY RAND() LIMIT 1").FirstOrDefault();
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

    public static bool HasTooManyRemindersSet(string target, bool isTimedReminder, OkayegTeaTimeContext database = null)
    {
        database ??= new();
        if (!isTimedReminder)
        {
            return database.Reminders.AsQueryable().Count(r => r.ToUser == target && r.ToTime == 0) >= AppSettings.MaxReminders;
        }
        else
        {
            return database.Reminders.AsQueryable().Count(r => r.ToUser == target && r.ToTime > 0) >= AppSettings.MaxReminders;
        }
    }

    public static bool IsEmoteManagementSub(string channel)
    {
        return new OkayegTeaTimeContext().Channels.FirstOrDefault(c => c.ChannelName == channel.ToLower()).EmoteManagementSub == true;
    }

    public static void RemoveReminder(ITwitchChatMessage chatMessage)
    {
        using var database = new OkayegTeaTimeContext();
        Reminder reminder = database.Reminders.FirstOrDefault(r => r.Id == chatMessage.Split[2].ToInt());
        if (reminder is not null)
        {
            if (reminder.FromUser == chatMessage.Username
                || (reminder.ToUser == chatMessage.Username && reminder.ToTime != 0)
                || AppSettings.UserLists.Moderators.Contains(chatMessage.UserId))
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
        SetAfkStatus(username, true);
    }

    public static void SetAfk(ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        using var database = new OkayegTeaTimeContext();
        User user = database.Users.FirstOrDefault(u => u.Username == chatMessage.Username);
        string message = chatMessage.Split.Length > 1 ? chatMessage.Split[1..].JoinToString(' ') : null;
        user.MessageText = message?.Encode();
        user.Type = type.ToString();
        user.Time = TimeHelper.Now();
        database.SaveChanges();
        SetAfkStatus(chatMessage.Username, true);
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

    private static void SetAfkStatus(string username, bool afk)
    {
        using var database = new OkayegTeaTimeContext();
        database.Users.AsQueryable().Where(u => u.Username == username).FirstOrDefault().IsAfk = afk;
        database.SaveChanges();
    }
}
