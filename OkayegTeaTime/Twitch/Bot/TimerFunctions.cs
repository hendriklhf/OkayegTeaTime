using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Api;

namespace OkayegTeaTime.Twitch.Bot;

public static class TimerFunctions
{
    public static void CheckForTimedReminders(TwitchBot twitchBot)
    {
        IEnumerable<Reminder> reminders = DbControl.Reminders.GetExpiredReminders();
        reminders.ForEach(twitchBot.SendTimedReminder);
    }

    public static void SetConsoleTitle(TwitchBot twitchBot)
    {
        Console.Title = $"OkayegTeaTime - {twitchBot.SystemInfo}";
    }

    public static void TwitchApiRefreshAccessToken()
    {
        TwitchApi.RefreshAccessToken();
    }
}
