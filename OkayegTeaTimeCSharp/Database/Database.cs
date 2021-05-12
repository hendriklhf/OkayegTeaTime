using Microsoft.EntityFrameworkCore;
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
            if (database.Nukes.Any(n => n.Channel == $"#{chatMessage.Channel}"))
            {
                List<Nuke> listNukes = database.Nukes.Where(n => n.Channel == $"#{chatMessage.Channel}").ToList();
                listNukes.ForEach(n =>
                {
                    if (n.ForTime > TimeHelper.Now())
                    {
                        if (chatMessage.GetMessage().IsMatch(n.Word.Decode()))
                        {
                            twitchBot.Timeout(chatMessage.Channel, chatMessage.Username, n.TimeoutTime, Nuke.Reason);
                        }
                    }
                    else
                    {
                        database.Nukes.Remove(n);
                        database.SaveChanges();
                    }
                });
            }
        }

        public static void InsertNewUser(string username)
        {
            OkayegTeaTimeContext database = new();
            if (!database.Users.Any(u => u.Username == username))
            {
                database.AddUser(username);
            }
        }

        public static Dictionary<string, string> GetPrefixes()
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.ToDictionary(p => p.Channel, prefix => prefix.PrefixString);
        }

        public static string GetPrefix(string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Prefixes.Where(p => p.Channel == $"#{channel}").FirstOrDefault().PrefixString;
        }

        public static Gachi GetRandomGachi()
        {
            OkayegTeaTimeContext database = new();
            return database.Gachi.OrderBy(g => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Pechkekse GetRandomCookie()
        {
            OkayegTeaTimeContext database = new();
            return database.Pechkekse.OrderBy(p => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Yourmom GetRandomYourmom()
        {
            OkayegTeaTimeContext database = new();
            return database.Yourmom.OrderBy(y => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Message GetRandomMessage(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Channel == $"#{chatMessage.Channel}").OrderBy(m => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Message GetRandomMessage(string username)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Username == username).OrderBy(m => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Message GetRandomMessage(string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Channel == $"#{channel.Replace("#", "")}" && m.Username == username).OrderBy(m => Guid.NewGuid()).Take(1).FirstOrDefault();
        }

        public static Message GetFirstMessageUserChannel(string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Username == username && channel == $"#{channel.Replace("#", "")}").FirstOrDefault();
        }

        public static Message GetFirstChannel(ChatMessage chatMessage, string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Username == chatMessage.Username && m.Channel == $"#{channel.Replace("#", "")}").FirstOrDefault();
        }

        public static Message GetFirstUser(string username)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Username == username).FirstOrDefault();
        }

        public static Message GetFirst(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.Where(m => m.Username == chatMessage.Username).FirstOrDefault();
        }

        public static Message GetSearchUserChannel(string keyword, string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Username = '{username}' AND Channel = '#{channel.Replace("#", "").MakeQueryable()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static Message GetSearchUser(string keyword, string username)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Username = '{username}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static Message GetSearchChannel(string keyword, string channel)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Channel = '#{channel.Replace("#", "").MakeQueryable()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static Message GetSearch(string keyword)
        {
            OkayegTeaTimeContext database = new();
            return database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }
    }
}
