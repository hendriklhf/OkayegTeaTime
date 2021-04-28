using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Messages;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DataBase
    {
        public static void LogMessage(ChatMessage chatMessage)
        {
            #warning keine nachrichten von bots etc. loggen
            OkayegTeaTimeContext database = new();
            database.Messages.Add(new Message(chatMessage.Username, chatMessage.Message.Transform(), chatMessage.Channel, TimeHelper.Now()));
            database.SaveChanges();
        }

        public static void CheckIfAFK(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.Where(user => user.Username == chatMessage.Username).FirstOrDefault();
            if (user.IsAfk == "true")
            {
                twitchBot.SendComingBack(chatMessage, user);
                DatabaseHelper.SetAfk(user, "false");
                #warning nicht false setzen, wenn wieder ein afk command ausgeführt wird
            }
        }

        public static void CheckForTimedReminder(TwitchBot twitchBot)
        {
            OkayegTeaTimeContext database = new();
            if (database.Reminders.Any(reminder => reminder.ToTime != 0))
            {
                List<Reminder> listReminder = database.Reminders.Where(reminder => reminder.ToTime != 0).ToList();
                listReminder.ForEach(reminder =>
                {
                    if (reminder.ToTime <= TimeHelper.Now())
                    {
                        twitchBot.SendTimedReminder(reminder);
                        database.Reminders.Remove(reminder);
                        database.SaveChanges();
                    }
                });
            }
        }
    }
}
