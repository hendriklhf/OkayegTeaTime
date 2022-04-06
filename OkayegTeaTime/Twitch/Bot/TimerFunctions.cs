using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Api;

namespace OkayegTeaTime.Twitch.Bot;

public static class TimerFunctions
{
    public static void CheckForTimedReminders(TwitchBot twitchBot)
    {
        DbController.CheckForTimedReminder(twitchBot);
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
