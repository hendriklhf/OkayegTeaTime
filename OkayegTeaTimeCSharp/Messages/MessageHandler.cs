using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.LogMessage(chatMessage);

            DataBase.CheckIfAFK(twitchBot, chatMessage);

            DataBase.CheckForReminder(twitchBot, chatMessage.Username);

            DataBase.InsertNewUser(chatMessage.Username);

            CommandHandler.Handle(twitchBot, chatMessage);
        }
    }
}
