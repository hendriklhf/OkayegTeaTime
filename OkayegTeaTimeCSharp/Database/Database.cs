using OkayegTeaTimeCSharp.Bot;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Time;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DataBase
    {
        public static void LogMessage(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            database.Messages.Add(new Message(chatMessage.Username, chatMessage.Message, chatMessage.Channel, TimeHelper.Now()));
            database.SaveChanges();
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
