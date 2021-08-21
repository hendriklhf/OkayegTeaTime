using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Twitch.API;
using System;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class TimerFunctions
    {
        public static void CheckForTimedReminders(TwitchBot twitchBot)
        {
            DataBase.CheckForTimedReminder(twitchBot);
        }

        public static void SetConsoleTitle(TwitchBot twitchBot)
        {
            Console.Title = $"OkayegTeaTime - {twitchBot.GetSystemInfo()}";
        }

        public static void TwitchApiRefreshAccessToken()
        {
            new TwitchAPI().RefreshAccessToken();
        }
    }
}