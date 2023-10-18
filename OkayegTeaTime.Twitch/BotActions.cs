using System;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public static class BotActions
{
    private const string _yourself = "yourself";
    private const string _reminderFromSpace = "reminder from ";

    public static async ValueTask SendComingBackAsync(this TwitchBot twitchBot, long userId, string channel)
    {
        User? user = twitchBot.Users[userId];
        if (user is null)
        {
            return;
        }

        await SendComingBackAsync(twitchBot, user, channel);
    }

    public static async ValueTask SendComingBackAsync(this TwitchBot twitchBot, User user, string channel)
    {
        string emote = twitchBot.Channels[channel]?.Emote ?? AppSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(AppSettings.MaxMessageLength);
        responseBuilder.Append(emote, " ");

        int afkMessageLength = twitchBot.AfkMessageBuilder.BuildComingBackMessage(user, user.AfkType, responseBuilder.FreeBufferSpan);
        responseBuilder.Advance(afkMessageLength);
        await twitchBot.SendAsync(channel, responseBuilder.WrittenMemory);
    }

    public static async ValueTask SendReminderAsync(this TwitchBot twitchBot, string channel, Reminder[] reminders)
    {
        if (reminders.Length == 0)
        {
            return;
        }

        string creator = reminders[0].Creator == reminders[0].Target ? _yourself : reminders[0].Creator;
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminders[0].Time);

        using PooledStringBuilder builder = new(500);
        builder.Append(reminders[0].Target, ", ", _reminderFromSpace, creator, " (");
        builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
        builder.Append(" ago)");

        twitchBot.Reminders.Remove(reminders[0].Id);
        if (reminders[0].Message?.Length > 0)
        {
            builder.Append(": ", reminders[0].Message);
        }

        for (int i = 1; i < reminders.Length; i++)
        {
            Reminder reminder = reminders[i];
            twitchBot.Reminders.Remove(reminder.Id);
            creator = reminder.Creator == reminder.Target ? _yourself : reminder.Creator;
            timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);

            builder.Append(" || ", creator, " (");
            builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
            builder.Append(" ago)");

            if (reminder.Message?.Length > 0)
            {
                builder.Append(": ", reminder.Message);
            }
        }

        await twitchBot.SendAsync(channel, builder.ToString());
    }

    public static async ValueTask SendTimedReminderAsync(this TwitchBot twitchBot, Reminder reminder)
    {
        string creator = reminder.Target == reminder.Creator ? _yourself : reminder.Creator;
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);

        using PooledStringBuilder builder = new(500);
        builder.Append(reminder.Target, ", ", _reminderFromSpace, creator, " (");
        builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
        builder.Append(" ago)");

        if (!string.IsNullOrWhiteSpace(reminder.Message))
        {
            builder.Append(": ", reminder.Message);
        }

        twitchBot.Reminders.Remove(reminder.Id);
        await twitchBot.SendAsync(reminder.Channel, builder.ToString());
    }
}
