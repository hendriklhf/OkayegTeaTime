using System;
#if DEBUG
using System.IO;
#endif
using System.Linq;
using System.Text;
#if DEBUG
using System.Text.Json;
#endif
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
            user.Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        database.SaveChanges();
        return entry?.Entity.Id;
    }

    public static int AddReminder(Reminder reminder)
    {
        using OkayegTeaTimeContext database = new();
        EntityEntry<Reminder> entry = database.Reminders.Add(reminder);
        database.SaveChanges();
        return entry.Entity.Id > 0 ? entry.Entity.Id : -1;
    }

    public static void AddSuggestion(string username, string channel, string suggestion)
    {
        using OkayegTeaTimeContext database = new();
        database.Suggestions.Add(new(username, Encoding.UTF8.GetBytes(suggestion), channel));
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

    public static void LogException(Exception ex)
    {
        ExceptionLog log = new(ex);
#if DEBUG
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
    ///     Removes a reminder without checking for permission.
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

        if (reminder.Creator != username && (reminder.Target != username || reminder.ToTime == 0) && !AppSettings.UserLists.Moderators.Contains(userId))
        {
            return false;
        }

        database.Reminders.Remove(reminder);
        database.SaveChanges();
        return true;
    }
}
