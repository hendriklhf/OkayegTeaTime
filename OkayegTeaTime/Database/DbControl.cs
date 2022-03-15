using OkayegTeaTime.Database.Cache;

namespace OkayegTeaTime.Database;

public static class DbControl
{
    public static ChannelCache Channels { get; } = new();

    public static ReminderCache Reminders { get; } = new();

    //public static SpotifyUserCache SpotifyUsers { get; } = new();

    public static UserCache Users { get; } = new();
}
