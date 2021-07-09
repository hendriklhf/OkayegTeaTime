using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Twitch.API;
using System;
using static OkayegTeaTimeCSharp.Program;

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

        public static void ConnectionStatus(TwitchBot twitchBot)
        {
            ConsoleOut($"TwitchClient: {twitchBot.TwitchClient.IsConnected}", fontColor: ConsoleColor.Red);
            ConsoleOut($"TcpClient: {twitchBot.TcpClient.IsConnected}", fontColor: ConsoleColor.Red);
        }

        public static void SetConsoleTitle(TwitchBot twitchBot)
        {
            Console.Title = $"OkayegTeaTime - {twitchBot.GetSystemInfo()}";
        }

        public static void TwitchApiRefreshAccessToken()
        {
            TwitchAPI.RefreshAccessToken();
        }
    }
}