using OkayegTeaTimeCSharp.Database;
using System;

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
            throw new NotImplementedException();
        }
    }
}
