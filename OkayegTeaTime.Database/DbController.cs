using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HLE;
using HLE.Time;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OkayegTeaTime.Database.EntityFrameworkModels;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Database;

public static class DbController
{
    public static void AddChannel(long id, string channel)
    {
        // FIXME: all operations create a Context, act on it and dispose straight away
        // Would be better to make this class non-static (treat as a Repository) & pool connections (see the following:
        // https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics#dbcontext-pooling)
        using OkayegTeaTimeContext database = new();
        database.Channels.Add(new(id, channel));
        database.SaveChanges();
    }

    public static void AddUser(User user, bool checkIfUserExists = false)
    {
        User? efUser;
        if (checkIfUserExists)
        {
            efUser = GetUser(user.Id, user.Username);
            if (efUser is not null)
            {
                return;
            }
        }

        OkayegTeaTimeContext database = new();
        efUser = new(user.Id, user.Username)
        {
            AfkType = user.AfkType
        };
        database.Users.Add(efUser);
        database.SaveChanges();
    }

    public static long? AddSpotifyUser(string username, string accessToken, string refreshToken)
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

    public static int AddReminder(Reminder reminder)
    {
        using OkayegTeaTimeContext database = new();
        if (HasTooManyRemindersSet(reminder.Target, reminder.ToTime > 0))
        {
            return -1;
        }

        EntityEntry<Reminder> entry = database.Reminders.Add(reminder);
        database.SaveChanges();
        return entry.Entity.Id;
    }

    public static int[] AddReminders(IEnumerable<Reminder> reminders)
    {
        Reminder[] rmdrs = reminders.ToArray();
        EntityEntry<Reminder>?[] entries = new EntityEntry<Reminder>[rmdrs.Length];
        using OkayegTeaTimeContext database = new();
        for (int i = 0; i < rmdrs.Length; i++)
        {
            if (HasTooManyRemindersSet(rmdrs[i].Target, rmdrs[i].ToTime > 0))
            {
                entries[i] = null;
            }
            else
            {
                entries[i] = database.Reminders.Add(rmdrs[i]);
            }
        }

        database.SaveChanges();
        return entries.Select(e => e?.Entity?.Id ?? -1).ToArray();
    }

    public static void AddSugestion(string username, string channel, string suggestion)
    {
        using OkayegTeaTimeContext database = new();
        database.Suggestions.Add(new(username, suggestion.Encode(), channel));
        database.SaveChanges();
    }

    public static Channel? GetChannel(string channel)
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.FirstOrDefault(c => c.Name == channel);
    }

    public static Channel? GetChannel(long id)
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.FirstOrDefault(c => c.Id == id);
    }

    public static Channel[] GetChannels()
    {
        using OkayegTeaTimeContext database = new();
        return database.Channels.ToArray();
    }

    public static Reminder? GetReminder(int id)
    {
        using OkayegTeaTimeContext database = new();
        return database.Reminders.FirstOrDefault(r => r.Id == id);
    }

    public static Reminder[] GetReminders()
    {
        using OkayegTeaTimeContext database = new();
        return database.Reminders.ToArray();
    }

    public static EntityFrameworkModels.Spotify? GetSpotifyUser(string username)
    {
        using OkayegTeaTimeContext database = new();
        return database.Spotify.FirstOrDefault(s => s.Username == username);
    }

    public static EntityFrameworkModels.Spotify[] GetSpotifyUsers()
    {
        using OkayegTeaTimeContext database = new();
        return database.Spotify.ToArray();
    }

    public static User? GetUser(long userId, string? username = null)
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

    public static User[] GetUsers()
    {
        using OkayegTeaTimeContext database = new();
        return database.Users.ToArray();
    }

    public static bool HasTooManyRemindersSet(string target, bool isTimedReminder)
    {
        using OkayegTeaTimeContext database = new();
        if (!isTimedReminder)
        {
            return database.Reminders.Count(r => r.Target == target && r.ToTime == 0) >= AppSettings.MaxReminders;
        }
        else
        {
            return database.Reminders.Count(r => r.Target == target && r.ToTime > 0) >= AppSettings.MaxReminders;
        }
    }

    public static void LogException(Exception ex)
    {
#if DEBUG
        ExceptionLog log = new(ex);
        File.WriteAllText($"exception_{Guid.NewGuid()}", JsonSerializer.Serialize(log, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
#elif RELEASE
        using OkayegTeaTimeContext database = new();
        database.ExceptionLogs.Add(log);
        database.SaveChanges();
#endif
    }

    public static void RemoveChannel(long id)
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

    public static bool RemoveReminder(long userId, string username, int reminderId)
    {
        using OkayegTeaTimeContext database = new();
        Reminder? reminder = database.Reminders.FirstOrDefault(r => r.Id == reminderId);
        if (reminder is null)
        {
            return false;
        }

        if (reminder.Creator == username || (reminder.Target == username && reminder.ToTime != 0) || AppSettings.UserLists.Moderators.Contains(userId))
        {
            database.Reminders.Remove(reminder);
            database.SaveChanges();
            return true;
        }

        return false;
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
