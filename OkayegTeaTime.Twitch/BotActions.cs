using System;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public static class BotActions
{
    private const string Yourself = "yourself";
    private const string ReminderFromSpace = "reminder from ";

    // ReSharper disable once InconsistentNaming
    public static ValueTask SendComingBackAsync(this TwitchBot twitchBot, long userId, string channel)
    {
        User? user = twitchBot.Users[userId];
        return user is null ? ValueTask.CompletedTask : SendComingBackAsync(twitchBot, user, channel);
    }

    public static async ValueTask SendComingBackAsync(this TwitchBot twitchBot, User user, string channel)
    {
        string emote = twitchBot.Channels[channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
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

        string creator = reminders[0].Creator == reminders[0].Target ? Yourself : reminders[0].Creator;
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminders[0].Time);

        using PooledStringBuilder builder = new(500);
        builder.Append(reminders[0].Target, ", ", ReminderFromSpace, creator, " (");
        builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
        builder.Append(" ago)");

        twitchBot.Reminders.Remove(reminders[0].Id);
        if (reminders[0].Message?.Length != 0)
        {
            builder.Append(": ", reminders[0].Message);
        }

        for (int i = 1; i < reminders.Length; i++)
        {
            Reminder reminder = reminders[i];
            twitchBot.Reminders.Remove(reminder.Id);
            creator = reminder.Creator == reminder.Target ? Yourself : reminder.Creator;
            timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);

            builder.Append(" || ", creator, " (");
            builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
            builder.Append(" ago)");

            if (reminder.Message?.Length != 0)
            {
                builder.Append(": ", reminder.Message);
            }
        }

        await twitchBot.SendAsync(channel, builder.ToString());
    }

    public static async ValueTask SendTimedReminderAsync(this TwitchBot twitchBot, Reminder reminder)
    {
        if (!twitchBot.IsConnectedTo(reminder.Channel))
        {
            return;
        }

        string creator = reminder.Target == reminder.Creator ? Yourself : reminder.Creator;
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);

        using PooledStringBuilder builder = new(512);
        builder.Append(reminder.Target, ", ", ReminderFromSpace, creator, " (");
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
