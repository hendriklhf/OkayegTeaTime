using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Database;

namespace OkayegTeaTimeCSharp.Commands
{
    public static class MessageHandler
    {
        public static void Handle(ChatMessage chatMessage)
        {
            DatabaseHelper.LogMessage(chatMessage);
        }
    }
}
