using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTimeCSharp.Database.Models;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.JavaScript;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DatabaseHelper
    {
        public static void LogMessage(this OkayegTeaTimeContext database, ChatMessage chatMessage)
        {
            database.Add(new Message(chatMessage.Username, chatMessage.Message.ToByteArray(), chatMessage.Channel, JavaScriptHelper.Now()));
            database.SaveChanges();
        }
    }
}
