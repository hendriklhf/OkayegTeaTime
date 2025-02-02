using System;
using System.Threading.Tasks;
using HLE.Text;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public partial class TwitchBot
{
    private const string Yourself = "yourself";
    private const string ReminderFromSpace = "reminder from ";

    public async ValueTask SendComingBackAsync(User user, string channel)
    {
        string emote = Channels[channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
        responseBuilder.Append(emote);
        responseBuilder.Append(' ');

        AfkMessageBuilder.BuildComingBackMessage(user, user.AfkType, responseBuilder);
        await SendAsync(channel, responseBuilder.WrittenMemory);
    }

    public async ValueTask SendReminderAsync(string channel, Reminder[] reminders)
    {
        if (reminders.Length == 0)
        {
            return;
        }

        Reminder reminder = reminders[0];
        string creator = reminder.Creator == reminder.Target ? Yourself : reminder.Creator;

#pragma warning disable S6354
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
#pragma warning restore S6354

        using PooledStringBuilder builder = new(500);
        builder.Append($"{reminder.Target}, {ReminderFromSpace}{creator} (");
        TimeSpanFormatter.Format(timeSinceReminderCreation, builder);
        builder.Append(" ago)");

        Reminders.Remove(reminder.Id);

        if (reminder.Message is { Length: not 0 })
        {
            builder.Append(": ");
            builder.Append(reminder.Message);
        }

        for (int i = 1; i < reminders.Length; i++)
        {
            reminder = reminders[i];
            Reminders.Remove(reminder.Id);
            creator = reminder.Creator == reminder.Target ? Yourself : reminder.Creator;
#pragma warning disable S6354
            timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
#pragma warning restore S6354

            builder.Append($" || {creator} (");
            TimeSpanFormatter.Format(timeSinceReminderCreation, builder);
            builder.Append(" ago)");

            if (reminder.Message is { Length: not 0 })
            {
                builder.Append(": ");
                builder.Append(reminder.Message);
            }
        }

        await SendAsync(channel, builder.ToString());
    }

    public async ValueTask SendTimedReminderAsync(Reminder reminder)
    {
        if (!IsConnectedTo(reminder.Channel))
        {
            return;
        }

        string creator = reminder.Target == reminder.Creator ? Yourself : reminder.Creator;
#pragma warning disable S6354
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
#pragma warning restore S6354

        using PooledStringBuilder builder = new(512);
        builder.Append($"{reminder.Target}, {ReminderFromSpace}{creator} (");
        TimeSpanFormatter.Format(timeSinceReminderCreation, builder);
        builder.Append(" ago)");

        if (reminder.Message is { Length: not 0 })
        {
            builder.Append(": ");
            builder.Append(reminder.Message);
        }

        Reminders.Remove(reminder.Id);
        await SendAsync(reminder.Channel, builder.ToString());
    }
}
