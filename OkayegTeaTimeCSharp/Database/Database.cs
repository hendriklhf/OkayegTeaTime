using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using Microsoft.EntityFrameworkCore;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Exceptions;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.CommandEnums;
using OkayegTeaTimeCSharp.Twitch.Messages;
using OkayegTeaTimeCSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Database
{
    public static class DataBase
    {
        public static void AddChannel(string channel)
        {
            OkayegTeaTimeContext database = new();
            database.Bots.FirstOrDefault(b => b.Id == 1).Channels += $" {channel.RemoveHashtag().Trim()}";
            database.SaveChanges();
        }

        public static void AddNewToken(string username, string accessToken, string refreshToken)
        {
            OkayegTeaTimeContext database = new();
            if (database.Spotify.Any(s => s.Username == username))
            {
                Models.Spotify user = database.Spotify.FirstOrDefault(s => s.Username == username);
                user.AccessToken = accessToken;
                user.RefreshToken = refreshToken;
                user.Time = TimeHelper.Now();
                database.SaveChanges();
            }
            else
            {
                database.Spotify.Add(new(username, accessToken, refreshToken));
                database.SaveChanges();
            }
        }

        public static int AddNuke(Nuke nuke)
        {
            OkayegTeaTimeContext database = new();
            database.Nukes.Add(nuke);
            database.SaveChanges();
            return database.Nukes.FirstOrDefault(n =>
                n.Channel == nuke.Channel
                && n.ForTime == nuke.ForTime
                && n.TimeoutTime == nuke.TimeoutTime
                && n.Username == nuke.Username
                && n.Word == nuke.Word).Id;
        }

        public static int AddReminder(Reminder reminder)
        {
            OkayegTeaTimeContext database = new();
            if (reminder.ToTime == 0)
            {
                if (database.Reminders.Where(r => r.ToUser == reminder.ToUser && r.ToTime == 0).Count() >= Config.MaxReminders)
                {
                    throw new TooManyReminderException();
                }
            }
            else
            {
                if (database.Reminders.Where(r => r.ToUser == reminder.ToUser && r.ToTime != 0).Count() >= Config.MaxReminders)
                {
                    throw new TooManyReminderException();
                }
            }
            database.Reminders.Add(reminder);
            database.SaveChanges();
            return database.Reminders.FirstOrDefault(r => r.FromUser == reminder.FromUser && r.ToUser == reminder.ToUser && r.Message == reminder.Message && r.ToTime == reminder.ToTime && r.Time == reminder.Time).Id;
        }

        public static void AddSugestion(ChatMessage chatMessage, string suggestion)
        {
            OkayegTeaTimeContext database = new();
            database.Suggestions.Add(new(chatMessage.Username, suggestion.MakeInsertable(), $"#{chatMessage.Channel}"));
            database.SaveChanges();
        }

        public static void CheckForNukes(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            if (!chatMessage.IsAnyCommand())
            {
                OkayegTeaTimeContext database = new();
                if (database.Nukes.Any(n => n.Channel == $"#{chatMessage.Channel}"))
                {
                    database.Nukes.Where(n => n.Channel == $"#{chatMessage.Channel}").ForEach(n =>
                    {
                        if (n.ForTime > TimeHelper.Now())
                        {
                            if (!chatMessage.IsModOrBroadcaster())
                            {
                                if (chatMessage.GetMessage().IsMatch(n.Word.Decode()))
                                {
                                    twitchBot.Timeout(chatMessage.Channel, chatMessage.Username, n.TimeoutTime, Nuke.Reason);
                                }
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
                List<Reminder> listReminder = database.Reminders.Where(reminder => reminder.ToTime != 0 && reminder.ToTime <= TimeHelper.Now()).ToList();
                listReminder.ForEach(reminder =>
                {
                    twitchBot.SendTimedReminder(reminder);
                    database.Reminders.Remove(reminder);
                });
                database.SaveChanges();
            }
        }

        public static void CheckIfAFK(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.FirstOrDefault(user => user.Username == chatMessage.Username);
            if (user.IsAfk == true)
            {
                twitchBot.SendComingBack(user, chatMessage);
                if (!chatMessage.IsAfkCommand())
                {
                    database.SetAfk(chatMessage.Username, false);
                }
            }
        }

        public static List<string> GetChannels()
        {
            return new OkayegTeaTimeContext().Bots.FirstOrDefault(b => b.Username == Resources.Username).Channels.Split().ToList();
        }

        public static Dictionary<string, string> GetEmotesInFront()
        {
            return new OkayegTeaTimeContext().EmoteInFronts.ToDictionary(e => e.Channel, e => e.Emote?.Decode());
        }

        public static Message GetFirst(ChatMessage chatMessage)
        {
            try
            {
                OkayegTeaTimeContext database = new();
                return database.Messages.FirstOrDefault(m => m.Username == chatMessage.Username);
            }
            catch (Exception)
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetFirstChannel(ChatMessage chatMessage, string channel)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FirstOrDefault(m => m.Username == chatMessage.Username && m.Channel == $"#{channel.RemoveHashtag()}");
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetFirstMessageUserChannel(string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FirstOrDefault(m => m.Username == username && channel == $"#{channel.RemoveHashtag()}");
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetFirstUser(string username)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FirstOrDefault(m => m.Username == username);
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetLastMessage(string username)
        {

            OkayegTeaTimeContext database = new();
            Message message = database.Messages.Where(m => m.Username == username).OrderByDescending(m => m.Id).FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new UserNotFoundException();
            }
        }

        public static string GetPrefix(string channel)
        {
            return new OkayegTeaTimeContext().Prefixes.FirstOrDefault(p => p.Channel == $"#{channel.RemoveHashtag()}").PrefixString?.Decode();
        }

        public static Dictionary<string, string> GetPrefixes()
        {
            return new OkayegTeaTimeContext().Prefixes.ToDictionary(p => p.Channel, p => p.PrefixString?.Decode());
        }

        public static Pechkekse GetRandomCookie()
        {
            OkayegTeaTimeContext database = new();
            return database.Pechkekse.FromSqlRaw($"SELECT * FROM pechkekse ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static Gachi GetRandomGachi()
        {
            OkayegTeaTimeContext database = new();
            return database.Gachi.FromSqlRaw($"SELECT * FROM gachi ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static Message GetRandomMessage(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE channel = '#{chatMessage.Channel}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetRandomMessage(string username)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE username = '{username.MakeQueryable()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetRandomMessage(string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE username ='{username.MakeQueryable()}' AND channel = '#{channel.MakeQueryable()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Yourmom GetRandomYourmom()
        {
            OkayegTeaTimeContext database = new();
            return database.Yourmom.FromSqlRaw($"SELECT * FROM yourmom ORDER BY RAND() LIMIT 1").FirstOrDefault();
        }

        public static string GetRefreshToken(string username)
        {
            return new OkayegTeaTimeContext().Spotify.FirstOrDefault(s => s.Username == username).RefreshToken;
        }

        public static Reminder GetReminder(int id)
        {
            Reminder reminder = new OkayegTeaTimeContext().Reminders.FirstOrDefault(r => r.Id == id);
            if (reminder != null)
            {
                return reminder;
            }
            else
            {
                throw new ReminderNotFoundException();
            }
        }

        public static Message GetSearch(string keyword)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetSearchChannel(string keyword, string channel)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Channel = '#{channel.RemoveHashtag().MakeQueryable().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetSearchUser(string keyword, string username)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Username = '{username.MakeQueryable().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Message GetSearchUserChannel(string keyword, string username, string channel)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FromSqlRaw($"SELECT * FROM messages WHERE CONVERT(MessageText USING latin1) LIKE '%{keyword.MakeQueryable()}%' AND Username = '{username.MakeQueryable().ToLower()}' AND Channel = '#{channel.RemoveHashtag().MakeQueryable().ToLower()}' ORDER BY RAND() LIMIT 1").FirstOrDefault();
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
            }
        }

        public static Models.Spotify GetSpotifyUser(string username)
        {
            return new OkayegTeaTimeContext().Spotify.FirstOrDefault(s => s.Username == username);
        }

        public static User GetUser(string username)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                return user;
            }
            else
            {
                throw new UserNotFoundException();
            }
        }

        public static Message GetMessage(int id)
        {
            OkayegTeaTimeContext database = new();
            Message message = database.Messages.FirstOrDefault(m => m.Id == id);
            if (message != null)
            {
                return message;
            }
            else
            {
                throw new MessageNotFoundException();
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

        public static void LogMessage(ChatMessage chatMessage)
        {
            if (!Config.NotLoggedChannels.Contains(chatMessage.Channel))
            {
                OkayegTeaTimeContext database = new();
                database.Messages.Add(new(chatMessage.Username, chatMessage.Message.MakeInsertable(), chatMessage.Channel));
                database.SaveChanges();
            }
        }

        public static void RemoveNuke(ChatMessage chatMessage)
        {
            int id = chatMessage.GetSplit()[2].ToInt();
            OkayegTeaTimeContext database = new();
            Nuke nuke = database.Nukes.FirstOrDefault(n => n.Id == id && n.Channel == $"#{chatMessage.Channel.RemoveHashtag()}");
            if (nuke != null)
            {
                if (chatMessage.IsModOrBroadcaster())
                {
                    database.Nukes.Remove(nuke);
                    database.SaveChanges();
                }
                else
                {
                    throw new NoPermissionException("you aren't a mod or the broadcaster");
                }
            }
            else
            {
                throw new NukeNotFoundException();
            }
        }

        public static void RemoveReminder(ChatMessage chatMessage)
        {
            OkayegTeaTimeContext database = new();
            Reminder reminder = database.Reminders.FirstOrDefault(r => r.Id == chatMessage.GetSplit()[2].ToInt());
            if (reminder != null)
            {
                if (reminder.FromUser == chatMessage.Username || (reminder.ToUser == chatMessage.Username && reminder.ToTime != 0))
                {
                    database.Reminders.Remove(reminder);
                    database.SaveChanges();
                }
                else
                {
                    throw new NoPermissionException("you have no permission to delete the reminder of someone else");
                }
            }
            else
            {
                throw new ReminderNotFoundException();
            }
        }

        public static void ResumeAfkStatus(string username)
        {
            OkayegTeaTimeContext database = new();
            database.SetAfk(username, true);
            database.SaveChanges();
        }

        public static void SetAfk(ChatMessage chatMessage, AfkCommandType type)
        {
            OkayegTeaTimeContext database = new();
            User user = database.Users.FirstOrDefault(u => u.Username == chatMessage.Username);
            string message = chatMessage.GetSplit().Length > 1 ? chatMessage.GetSplit()[1..].ToSequence() : string.Empty;
            user.MessageText = message.MakeInsertable();
            user.Type = type.ToString();
            user.Time = TimeHelper.Now();
            database.SaveChanges();
            database.SetAfk(chatMessage.Username, true);
        }

        public static void SetEmoteInFront(string channel, string emote)
        {
            OkayegTeaTimeContext database = new();
            if (database.EmoteInFronts.Any(e => e.Channel == $"#{channel.RemoveHashtag()}"))
            {
                database.EmoteInFronts.FirstOrDefault(e => e.Channel == $"#{channel.RemoveHashtag()}").Emote = emote.MakeInsertable();
                database.SaveChanges();
            }
            else
            {
                database.EmoteInFronts.Add(new(channel, emote.MakeInsertable()));
                database.SaveChanges();
            }
        }

        public static void SetPrefix(string channel, string prefix)
        {
            OkayegTeaTimeContext database = new();
            if (database.Prefixes.Any(p => p.Channel == $"#{channel.RemoveHashtag()}"))
            {
                database.Prefixes.FirstOrDefault(p => p.Channel == $"#{channel.RemoveHashtag()}").PrefixString = prefix.MakeUsable().Encode();
                database.SaveChanges();
                PrefixHelper.Update(channel);
            }
            else
            {
                database.Prefixes.Add(new(channel, prefix.MakeUsable().Encode()));
                database.SaveChanges();
                PrefixHelper.Add(channel);
            }
        }

        public static void UnsetEmoteInFront(string channel)
        {
            OkayegTeaTimeContext database = new();
            if (database.EmoteInFronts.Any(e => e.Channel == $"#{channel.RemoveHashtag()}"))
            {
                database.EmoteInFronts.FirstOrDefault(e => e.Channel == $"#{channel.RemoveHashtag()}").Emote = null;
                database.SaveChanges();
            }
            else
            {
                database.EmoteInFronts.Add(new(channel, null));
                database.SaveChanges();
            }
        }

        public static void UnsetPrefix(string channel)
        {
            OkayegTeaTimeContext database = new();
            if (database.Prefixes.Any(p => p.Channel == $"#{channel.RemoveHashtag()}"))
            {
                database.Prefixes.FirstOrDefault(p => p.Channel == $"#{channel.RemoveHashtag()}").PrefixString = null;
                database.SaveChanges();
                PrefixHelper.Update(channel);
            }
            else
            {
                database.Prefixes.Add(new(channel, null));
                database.SaveChanges();
                PrefixHelper.Add(channel);
            }
        }

        public static void UpdateAccessToken(string username, string accessToken)
        {
            OkayegTeaTimeContext database = new();
            Models.Spotify user = database.Spotify.FirstOrDefault(s => s.Username == username);
            user.AccessToken = accessToken;
            user.Time = TimeHelper.Now();
            database.SaveChanges();
        }
    }
}
