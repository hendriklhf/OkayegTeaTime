using OkayegTeaTimeCSharp.Bot;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Database;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            DataBase.LogMessage(chatMessage);

            DataBase.CheckIfAFK(twitchBot, chatMessage);

            CommandHandler.Handle(chatMessage);
        }
    }
}
