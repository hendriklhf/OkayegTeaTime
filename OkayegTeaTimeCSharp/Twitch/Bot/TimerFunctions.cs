using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Twitch.API;
using System;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class TimerFunctions
    {
        public static void BanSecretChatUsers(TwitchBot twitchBot)
        {
        }

        public static void CheckForTimedReminders(TwitchBot twitchBot)
        {
            DataBase.CheckForTimedReminder(twitchBot);
        }

        public static void TwitchApiRefreshAccessToken()
        {
            TwitchAPI.RefreshAccessToken();
        }

        public static void SetConsoleTitle(TwitchBot twitchBot)
        {
            Console.Title = $"OkayegTeaTime - {twitchBot.GetSystemInfo()}";
        }
    }
}