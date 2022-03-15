using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OkayegTeaTime.Database.EntityFrameworkModels;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Database;

public static class DbController
{
    public static void AddChannel(int id, string channel)
    {
        // FIXME: all operations create a Context, act on it and dispose straight away
        // Would be better to make this class non-static (treat as a Repository) & pool connections (see the following:
        // https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#dbcontext-pooling)
        using OkayegTeaTimeContext database = new();
        database.Channels.Add(new(id, channel));
        database.SaveChanges();
    }

    public static void AddUser(int userId, string username, AfkCommandType type, bool checkIfUserExists = false)
    {
        _ = AddUserAndReturn(userId, username, type, checkIfUserExists);
    }

    public static User? AddUserAndReturn(int userId, string username, AfkCommandType type, bool checkIfUserExists = false)
    {
        User? user;
        if (checkIfUserExists)
        {
            user = GetUser(userId, username);
            if (user is not null)
            {
                return null;
            }
        }

        OkayegTeaTimeContext database = new();
        user = new(userId, username, type);
        user = database.Users.Add(user).Entity;
        database.SaveChanges();
        return user;
    }

    public static int? AddSpotifyUser(string username, string accessToken, string refreshToken)
    {
        using OkayegTeaTimeContext database = new();
        EntityFrameworkModels.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == username);

        EntityEntry<EntityFrameworkModels.Spotify>? entry = null;
        if (user is null)
        {
            entry = database.Spotify.Add(new(username, accessToken, refreshToken));
        }
        else
        {
            user.AccessToken = accessToken;
            user.RefreshToken = refreshToken;
            user.Time = TimeHelper.Now();
        }

        database.SaveChanges();
        return entry?.Entity.Id;
    }

    public static int? AddReminder(string creator, string target, string? message, string channel, long toTime = 0)
    {
        using OkayegTeaTimeContext database = new();
        Reminder reminder = new(creator, target, message?.Encode(), channel, toTime);

        if (HasTooManyRemindersSet(reminder.Target, reminder.ToTime > 0))
        {
            return null;
        }

        EntityEntry<Reminder> entity = database.Reminders.Add(reminder);
        database.SaveChanges();
        return entity.Entity.Id;
    }

    public static int?[] AddReminders(IEnumerable<(string Creator, string Target, string Message, string Channel, long ToTime)> rmdrs)
    {
        (string Creator, string Target, string Message, string Channel, long ToTime)[] reminders = rmdrs.ToArray();
        EntityEntry<Reminder>?[] entities = new EntityEntry<Reminder>[reminders.Length];
        using OkayegTeaTimeContext database = new();
        reminders.ForEach((v, i) =>
        {
            Reminder r = new(v);
            if (HasTooManyRemindersSet(r.Target, r.ToTime > 0))
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

    public static void AddSugestion(string username, string channel, string suggestion)
    {
        using OkayegTeaTimeContext database = new();
        database.Suggestions.Add(new(username, suggestion.Encode(), channel));
        database.SaveChanges();
    }

    //TODO: don't pass TwitchBot into here, don't delete reminders here
    [Obsolete("needs refactoring")]
    public static void CheckForReminder(TwitchBot twitchBot, TwitchChatMessage chatMessage)
    {
        using OkayegTeaTimeContext database = new();
        List<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime == 0 && r.Target == chatMessage.Username).ToList();
        if (!reminders.Any())
        {
            return;
        }

        twitchBot.SendReminder(chatMessage, reminders.Select(r => new Models.Reminder(r)));
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    //TODO: don't pass TwitchBot into here, don't delete reminders here
    [Obsolete("needs refactoring")]
    public static void CheckForTimedReminder(TwitchBot twitchBot)
    {
        using OkayegTeaTimeContext database = new();
        IEnumerable<Reminder> reminders = database.Reminders.AsQueryable().Where(r => r.ToTime != 0 && r.ToTime <= TimeHelper.Now());
        if (!reminders.Any())
        {
            return;
        }

        reminders.Select(r => new Models.Reminder(r)).ForEach(twitchBot.SendTimedReminder);
        database.Reminders.RemoveRange(reminders);
        database.SaveChanges();
    }

    public static Channel? GetChannel(string channel)
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.FirstOrDefault(c => c.Name == channel);
    }

    public static Channel? GetChannel(int id)
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.FirstOrDefault(c => c.Id == id);
    }

    public static List<Channel> GetChannels()
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.ToList();
    }

    public static Reminder? GetReminder(int id)
    {
        using OkayegTeaTimeContext database = new();
        return database.Reminders.FirstOrDefault(r => r.Id == id);
    }

    public static List<Reminder> GetReminders()
    {
        using OkayegTeaTimeContext database = new();
        return database.Reminders.ToList();
    }

    public static EntityFrameworkModels.Spotify? GetSpotifyUser(string username)
    {
        using OkayegTeaTimeContext database = new();
        return database.Spotify.FirstOrDefault(s => s.Username == username);
    }

    public static List<EntityFrameworkModels.Spotify> GetSpotifyUsers()
    {
        using OkayegTeaTimeContext database = new();
        return database.Spotify.ToList();
    }

    public static User? GetUser(int userId, string? username = null)
    {
        using OkayegTeaTimeContext database = new();
        User? user = database.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return user;
        }

        if (username is null || user.Username == username)
        {
            return user;
        }

        user.Username = username;
        database.SaveChanges();
        return user;
    }

    public static List<User> GetUsers()
    {
        using OkayegTeaTimeContext database = new();
        return database.Users.ToList();
    }

    public static bool HasTooManyRemindersSet(string target, bool isTimedReminder)
    {
        using OkayegTeaTimeContext database = new();
        if (!isTimedReminder)
        {
            return database.Reminders.AsQueryable().Count(r => r.Target == target && r.ToTime == 0) >= AppSettings.MaxReminders;
        }
        else
        {
            return database.Reminders.AsQueryable().Count(r => r.Target == target && r.ToTime > 0) >= AppSettings.MaxReminders;
        }
    }

    public static void LogException(Exception ex)
    {
        using OkayegTeaTimeContext database = new();
        ExceptionLog log = new(ex);
        database.ExceptionLogs.Add(log);
        database.SaveChanges();
    }

    public static void RemoveChannel(int id)
    {
        using OkayegTeaTimeContext database = new();
        Channel? chnl = database.Channels.FirstOrDefault(c => c.Id == id);
        if (chnl is null)
        {
            return;
        }

        database.Channels.Remove(chnl);
        database.SaveChanges();
    }

    public static void RemoveChannel(string channel)
    {
        using OkayegTeaTimeContext database = new();
        Channel? chnl = database.Channels.FirstOrDefault(c => c.Name == channel);
        if (chnl is null)
        {
            return;
        }

        database.Channels.Remove(chnl);
        database.SaveChanges();
    }

    /// <summary>
    /// Removes a reminder without checking for permission.
    /// </summary>
    public static void RemoveReminder(int reminderId)
    {
        using OkayegTeaTimeContext database = new();
        Reminder? reminder = database.Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return;
        }

        database.Reminders.Remove(reminder);
        database.SaveChanges();
    }

    public static bool RemoveReminder(int userId, string username, int reminderId)
    {
        using OkayegTeaTimeContext database = new();
        Reminder? reminder = database.Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return false;
        }

        if (reminder.Creator == username
            || reminder.Target == username && reminder.ToTime != 0
            || AppSettings.UserLists.Moderators.Contains(userId))
        {
            database.Reminders.Remove(reminder);
            database.SaveChanges();
            return true;
        }

        return false;
    }

    public static void SetSpotifyAccessToken(string username, string accessToken)
    {
        using OkayegTeaTimeContext database = new();
        EntityFrameworkModels.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == username);
        if (user is null)
        {
            return;
        }

        user.AccessToken = accessToken;
        database.SaveChanges();
    }

    public static bool SetSongRequestState(string username, bool enabled)
    {
        using OkayegTeaTimeContext database = new();
        EntityFrameworkModels.Spotify? user = database.Spotify.FirstOrDefault(s => s.Username == username);
        if (user is null)
        {
            return false;
        }

        user.SongRequestEnabled = enabled;
        database.SaveChanges();
        return true;
    }
}
