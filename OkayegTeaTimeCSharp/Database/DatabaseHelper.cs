using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Database.Models;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Utils;
using OkayegTeaTimeCSharp.Time;

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
