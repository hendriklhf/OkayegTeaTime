using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Database;

public static class DbController
{
    private static readonly DbContextOptions<OkayegTeaTimeContext> _options = new DbContextOptionsBuilder<OkayegTeaTimeContext>()
            .UseMySQL(AppSettings.DbConnection.ConnectionString)
            .Options;
    private static readonly PooledDbContextFactory<OkayegTeaTimeContext> _factory = new(_options);

    public static void AddChannel(string channel)
    {
        // FIXME: all operations create a Context, act on it and dispose straight away
        // Would be better to make this class non-static (treat as a Repository) & pool connections (see the following:
        // https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#dbcontext-pooling)
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        database.Channels.Add(new(channel));
        database.SaveChanges();
    }

    public static void AddNewToken(string username, string accessToken, string refreshToken)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == username);

        if (user is null)
        {
            database.Spotify.Add(new(username, accessToken, refreshToken));
            database.SaveChanges();
        }
        else
        {
            user.AccessToken = accessToken;
            user.RefreshToken = refreshToken;
            user.Time = TimeHelper.Now();
            database.SaveChanges();
        }
    }

    public static int? AddReminder(string fromUser, string toUser, string message, string channel, long toTime = 0)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
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
        EntityEntry<Reminder>?[] entities = new EntityEntry<Reminder>[count];
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        reminders.ForEach((v, i) =>
        {
            Reminder r = new(v);
            if (HasTooManyRemindersSet(r.ToUser, r.ToTime > 0))
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
        EntityEntry<Reminder>?[] entities = new EntityEntry<Reminder>[count];
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        reminders.ForEach((v, i) =>
        {
            Reminder r = new(v);
            if (HasTooManyRemindersSet(r.ToUser, r.ToTime > 0))
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

    public static void AddSugestion(TwitchChatMessage chatMessage, string suggestion)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        database.Suggestions.Add(new(chatMessage.Username, suggestion.Encode(), $"#{chatMessage.Channel}"));
        database.SaveChanges();
    }

    public static void AddUser(string username)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        if (!database.Users.Any(u => u.Username == username))
        {
            database.Users.Add(new(username));
            database.SaveChanges();
        }
    }

    public static void CheckForReminder(TwitchBot twitchBot, TwitchChatMessage chatMessage)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        List<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime == 0 && r.ToUser == chatMessage.Username).ToList();
        if (!reminders.Any())
        {
            return;
        }

        twitchBot.SendReminder(chatMessage, reminders);
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    public static void CheckForTimedReminder(TwitchBot twitchBot)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        IEnumerable<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime != 0 && r.ToTime <= TimeHelper.Now());
        if (!reminders.Any())
        {
            return;
        }

        reminders.ForEach(r => twitchBot.SendTimedReminder(r));
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    public static void CheckIfAfk(TwitchBot twitchBot, TwitchChatMessage chatMessage)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        User? user = database.Users.FirstOrDefault(u => u.Username == chatMessage.Username);

        if (user is null)
        {
            return;
        }

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
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Spotify.Any(s => s.Username == username.ToLower());
    }

    public static Models.Channel? GetChannel(string channel)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.FirstOrDefault(c => c.ChannelName == channel);
    }

    public static List<string> GetChannelNames()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.AsQueryable().Select(c => c.ChannelName).ToList();
    }

    public static List<Models.Channel> GetChannels()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.ToList();
    }

    public static string? GetEmoteInFront(string channel)
    {
        return GetChannel(channel)?.EmoteInFront?.Decode();
    }

    public static List<string> GetEmoteManagementSubs()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.AsQueryable().Where(c => c.EmoteManagementSub == true).Select(c => c.ChannelName).ToList();
    }

    public static Dictionary<string, string?> GetEmotesInFront()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.ToDictionary(c => c.ChannelName, c => c.EmoteInFront?.Decode());
    }

    public static string? GetPrefix(string channel)
    {
        return GetChannel(channel)?.Prefix?.Decode();
    }

    public static Dictionary<string, string?> GetPrefixes()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Channels.ToDictionary(c => c.ChannelName, c => c.Prefix?.Decode());
    }

    public static Yourmom? GetRandomYourmom()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Yourmom.FromSqlRaw($"SELECT * FROM yourmom ORDER BY RAND() LIMIT 1").FirstOrDefault();
    }

    public static List<Reminder> GetReminders()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Reminders.ToList();
    }

    public static string? GetRefreshToken(string username)
    {
        return GetSpotifyUser(username)?.RefreshToken;
    }

    public static Reminder? GetReminder(int id)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Reminders.FirstOrDefault(r => r.Id == id);
    }

    public static Models.Spotify? GetSpotifyUser(string username)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Spotify.FirstOrDefault(s => s.Username == username);
    }

    public static List<Models.Spotify> GetSpotifyUsers()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Spotify.ToList();
    }

    public static User? GetUser(string username)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Users.FirstOrDefault(u => u.Username == username);
    }

    public static List<User> GetUsers()
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        return database.Users.ToList();
    }

    public static bool HasTooManyRemindersSet(string target, bool isTimedReminder)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
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
        return GetChannel(channel)?.EmoteManagementSub == true;
    }

    public static bool RemoveReminder(TwitchChatMessage chatMessage, int reminderId)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Reminder? reminder = database.Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return false;
        }

        if (reminder.FromUser == chatMessage.Username
            || (reminder.ToUser == chatMessage.Username && reminder.ToTime != 0)
            || AppSettings.UserLists.Moderators.Contains(chatMessage.UserId))
        {
            database.Reminders.Remove(reminder);
            database.SaveChanges();
            return true;
        }
        return false;
    }

    public static void ResumeAfkStatus(string username)
    {
        SetAfkStatus(username, true);
    }

    public static void SetAfk(TwitchChatMessage chatMessage, AfkCommandType type)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        User? user = database.Users.FirstOrDefault(u => u.Username == chatMessage.Username);
        if (user is null)
        {
            return;
        }

        string? message = chatMessage.Split.Length > 1 ? chatMessage.Split[1..].JoinToString(' ') : null;
        user.MessageText = message?.Encode();
        user.Type = type.ToString();
        user.Time = TimeHelper.Now();
        database.SaveChanges();
        SetAfkStatus(chatMessage.Username, true);
    }

    public static void SetEmoteInFront(string channel, string emote)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Channel? chnl = database.Channels.FirstOrDefault(c => c.ChannelName == channel);
        if (chnl is not null)
        {
            chnl.EmoteInFront = emote.Encode();
            database.SaveChanges();
        }
    }

    public static void SetEmoteSub(string channel, bool subbed)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Channel? chnl = database.Channels.FirstOrDefault(c => c.ChannelName == channel);
        if (chnl is not null)
        {
            chnl.EmoteManagementSub = subbed;
            database.SaveChanges();
        }
    }

    public static void SetPrefix(string channel, string prefix)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Channel? chnl = database.Channels.FirstOrDefault(c => c.ChannelName == channel);
        if (chnl is not null)
        {
            chnl.Prefix = prefix.RemoveChatterinoChar().TrimAll().Encode();
            database.SaveChanges();
        }
    }

    public static void SetSongRequestEnabledState(string channel, bool enabled)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == channel.ToLower());
        if (user is null)
        {
            return;
        }

        user.SongRequestEnabled = enabled;
        database.SaveChanges();
    }

    public static void UnsetEmoteInFront(string channel)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Channel? chnl = database.Channels.FirstOrDefault(c => c.ChannelName == channel);
        if (chnl is not null)
        {
            chnl.EmoteInFront = null;
            database.SaveChanges();
        }
    }

    public static void UnsetPrefix(string channel)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Channel? chnl = database.Channels.FirstOrDefault(c => c.ChannelName == channel);
        if (chnl is not null)
        {
            chnl.Prefix = null;
            database.SaveChanges();
        }
    }

    public static void UpdateAccessToken(string username, string accessToken)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        Models.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == username);
        if (user is null)
        {
            return;
        }

        user.AccessToken = accessToken;
        user.Time = TimeHelper.Now();
        database.SaveChanges();
    }

    private static void SetAfkStatus(string username, bool afk)
    {
        using OkayegTeaTimeContext database = _factory.CreateDbContext();
        User? user = database.Users.FirstOrDefault(u => u.Username == username);
        if (user is null)
        {
            return;
        }

        user.IsAfk = afk;
        database.SaveChanges();
    }
}
