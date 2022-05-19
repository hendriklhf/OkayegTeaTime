using System.Reflection;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database;

public static class DbControl
{
    public static ChannelCache Channels { get; } = new();

    public static ReminderCache Reminders { get; } = new();

    public static SpotifyUserCache SpotifyUsers { get; } = new();

    public static UserCache Users { get; } = new();

    public static void Invalidate()
    {
        Channels.Invalidate();
        Reminders.Invalidate();
        Users.Invalidate();
    }

    public static IEnumerable<string> Invalidate(string cacheName)
    {
        Regex cachePattern = new($@"^{cacheName}$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        PropertyInfo[] properties = typeof(DbControl).GetProperties().Where(p => cachePattern.IsMatch(p.Name)).ToArray();
        foreach (PropertyInfo property in properties)
        {
            MethodInfo? method = property.PropertyType.GetMethod(nameof(DbCache<CacheModel>.Invalidate));
            method?.Invoke(property.GetValue(null), null);
        }

        return properties.Select(p => p.Name).ToArray();
    }
}
