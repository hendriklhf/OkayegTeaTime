using System.Text;
using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Bot;

public static class BotActions
{
    public static void SendComingBack(this TwitchBot twitchBot, User user, TwitchChatMessage chatMessage)
    {
        twitchBot.Send(chatMessage.Channel, new AfkMessage(user).ComingBack);
    }

    public static void SendGoingAfk(this TwitchBot twitchBot, TwitchChatMessage chatMessage, AfkCommandType type)
    {
        DbController.SetAfk(chatMessage, type);
        User? user = DbController.GetUser(chatMessage.Username);
        if (user is null)
        {
            DbController.AddUser(chatMessage.Username);
            user = DbController.GetUser(chatMessage.Username);
        }

        twitchBot.Send(chatMessage.Channel, new AfkMessage(user!).GoingAway);
    }

    public static void SendReminder(this TwitchBot twitchBot, TwitchChatMessage chatMessage, List<Reminder> reminders)
    {
        string message = $"{chatMessage.Username}, reminder from {reminders[0].GetAuthor()} ({TimeHelper.GetUnixDifference(reminders[0].Time)} ago)";
        StringBuilder builder = new(message);
        if (reminders[0].Message.Length > 0)
        {
            builder.Append($": {reminders[0].Message.Decode()}");
        }

        if (reminders.Count > 1)
        {
            reminders.Skip(1).ForEach(r =>
            {
                builder.Append($" || {r.GetAuthor()} ({TimeHelper.GetUnixDifference(r.Time)} ago)");
                if (r.Message.Length > 0)
                {
                    builder.Append($": {r.Message.Decode()}");
                }
            });
        }
        twitchBot.Send(chatMessage.Channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string message = $"{reminder.ToUser}, reminder from {reminder.GetAuthor()} ({TimeHelper.GetUnixDifference(reminder.Time)} ago)";
        string reminderMessage = reminder.Message.Decode();
        if (!string.IsNullOrEmpty(reminderMessage))
        {
            message += $": {reminderMessage}";
        }
        twitchBot.Send(reminder.Channel, message);
    }
}
