using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Twitch.Bot
{
    public static class TimerFunctions
    {
        public static void CheckForTimedReminders(TwitchBot twitchBot)
        {
            DataBase.CheckForTimedReminder(twitchBot);
        }

        public static void BanSecretChatUsers(TwitchBot twitchBot)
        {
#warning not implemented
        }
    }
}