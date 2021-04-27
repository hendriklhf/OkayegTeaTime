using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class MessageHandler
    {
        public static void Handle(ChatMessage chatMessage)
        {
            Database.DataBase.LogMessage(chatMessage);
        }
    }
}
