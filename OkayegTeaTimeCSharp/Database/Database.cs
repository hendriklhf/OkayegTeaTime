using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DataBase
    {
        public static void LogMessage(ChatMessage chatMessage)
        {
            if (!Config.GetNotLoggedChannels().Contains(chatMessage.Channel))
            {
                OkayegTeaTimeContext database = new();
                database.Messages.Add(new Message(chatMessage.Username, chatMessage.Message.MakeInsertable(), chatMessage.Channel));
                database.SaveChanges();
            }
        }

        public static void CheckIfAFK(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.Where(user => user.Username == chatMessage.Username).FirstOrDefault();
            if (user.IsAfk == "true")
            {
                twitchBot.SendComingBack(user, chatMessage);
                if (!MessageHelper.IsAfkCommand(chatMessage.GetMessage()))
                {
                    database.SetAfk(user, "false");
                }
            }
        }

        public static void CheckForReminder(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            if (database.Reminders.Any(reminder => reminder.ToTime == 0 && reminder.ToUser == chatMessage.Username))
            {
                List<Reminder> listReminder = database.Reminders.Where(reminder => reminder.ToTime == 0 && reminder.ToUser == chatMessage.Username).ToList();
                twitchBot.SendReminder(chatMessage, listReminder);
                database.RemoveReminder(listReminder);
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
                    }
                });
                database.SaveChanges();
            }
        }

        public static void CheckForNukes(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            if (database.Nukes.Any(nuke => nuke.Channel == $"#{chatMessage.Channel}"))
            {
                List<Nuke> listNukes = database.Nukes.Where(nuke => nuke.Channel == $"#{chatMessage.Channel}").ToList();
                listNukes.ForEach(nuke =>
                {
                    if (nuke.ForTime > TimeHelper.Now())
                    {
                        if (chatMessage.GetMessage().IsMatch(nuke.Word.ToString(1)))
                        {
                            twitchBot.Timeout(chatMessage.Channel, chatMessage.Username, nuke.TimeoutTime, Nuke.Reason);
                        }
                    }
                    else
                    {
                        database.Nukes.Remove(nuke);
                        database.SaveChanges();
                    }
                });
            }
        }

        public static void InsertNewUser(string username)
        {
            OkayegTeaTimeContext database = new();
            if (!database.Users.Any(user => user.Username == username))
            {
                database.AddUser(username);
            }
        }

        public static Dictionary<string, string> GetPrefixes()
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.ToDictionary(prefix => prefix.Channel, prefix => prefix.PrefixString);
        }

        public static string GetPrefix(string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.Where(prefix => prefix.Channel == $"#{channel}").FirstOrDefault().PrefixString;
        }

        public static Gachi GetRandomGachi()
        {
            OkayegTeaTimeContext database = new();
            Random rand = new();
            int skip = rand.Next(1, database.Gachi.Count());
            return database.Gachi.Skip(skip).Take(1).First();
        }
    }
}
