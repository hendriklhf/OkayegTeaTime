using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public static class BotActions
{
    public static void SendComingBack(this TwitchBot twitchBot, long userId, string channel)
    {
        User? user = twitchBot.Users[userId];
        if (user is null)
        {
            return;
        }

        AfkCommand cmd = twitchBot.CommandController[user.AfkType];
        string afkMessage = new AfkMessage(user, cmd).ComingBack;
        twitchBot.Send(channel, afkMessage);
    }

    public static void SendReminder(this TwitchBot twitchBot, string channel, IEnumerable<Reminder> reminders)
    {
        Reminder[] rmndrs = reminders.ToArray();
        if (rmndrs.Length == 0)
        {
            return;
        }

        string creator = rmndrs[0].Creator == rmndrs[0].Target ? "yourself" : rmndrs[0].Creator;
        TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(rmndrs[0].Time);
        string message = $"{rmndrs[0].Target}, reminder from {creator} ({span.Format()} ago)";
        StringBuilder builder = new(message);
        twitchBot.Reminders.Remove(rmndrs[0].Id);
        if (rmndrs[0].Message?.Length > 0)
        {
            builder.Append($": {rmndrs[0].Message}");
        }

        if (rmndrs.Length > 1)
        {
            foreach (Reminder r in rmndrs[1..])
            {
                twitchBot.Reminders.Remove(r.Id);
                creator = r.Creator == r.Target ? "yourself" : r.Creator;
                span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(r.Time);
                builder.Append($" || {creator} ({span.Format()} ago)");
                if (r.Message?.Length > 0)
                {
                    builder.Append($": {r.Message}");
                }
            }
        }

        twitchBot.Send(channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string creator = reminder.Target == reminder.Creator ? "yourself" : reminder.Creator;
        TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
        string message = $"{reminder.Target}, reminder from {creator} ({span.Format()} ago)";
        if (!string.IsNullOrEmpty(reminder.Message))
        {
            message += $": {reminder.Message}";
        }

        twitchBot.Reminders.Remove(reminder.Id);
        twitchBot.Send(reminder.Channel, message);
    }
}
