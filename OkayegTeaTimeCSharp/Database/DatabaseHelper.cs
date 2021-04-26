using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DatabaseHelper
    {
        public static void LogMessage(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            database.Add(new Message(chatMessage.Username, chatMessage.Message, chatMessage.Channel, TimeHelper.Now()));
            database.SaveChanges();
        }
    }
}
