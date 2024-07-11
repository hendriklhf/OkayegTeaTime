using System;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public partial class TwitchBot
{
    private const string Yourself = "yourself";
    private const string ReminderFromSpace = "reminder from ";

    // ReSharper disable once InconsistentNaming
    public ValueTask SendComingBackAsync(long userId, string channel)
    {
        User? user = Users[userId];
        return user is null ? ValueTask.CompletedTask : SendComingBackAsync(user, channel);
    }

    public async ValueTask SendComingBackAsync(User user, string channel)
    {
        string emote = Channels[channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
        responseBuilder.Append(emote, " ");

        int afkMessageLength = AfkMessageBuilder.BuildComingBackMessage(user, user.AfkType, responseBuilder.FreeBufferSpan);
        responseBuilder.Advance(afkMessageLength);
        await SendAsync(channel, responseBuilder.WrittenMemory);
    }

    public async ValueTask SendReminderAsync(string channel, Reminder[] reminders)
    {
        if (reminders.Length == 0)
        {
            return;
        }

        string creator = reminders[0].Creator == reminders[0].Target ? Yourself : reminders[0].Creator;
#pragma warning disable S6354
        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminders[0].Time);
#pragma warning restore S6354

        using PooledStringBuilder builder = new(500);
        builder.Append(reminders[0].Target, ", ", ReminderFromSpace, creator, " (");
        builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
        builder.Append(" ago)");

        Reminders.Remove(reminders[0].Id);
        if (reminders[0].Message?.Length != 0)
        {
            builder.Append(": ", reminders[0].Message);
        }

        for (int i = 1; i < reminders.Length; i++)
        {
            Reminder reminder = reminders[i];
            Reminders.Remove(reminder.Id);
            creator = reminder.Creator == reminder.Target ? Yourself : reminder.Creator;
#pragma warning disable S6354
            timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
#pragma warning restore S6354

            builder.Append(" || ", creator, " (");
            builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
            builder.Append(" ago)");

            if (reminder.Message?.Length != 0)
            {
                builder.Append(": ", reminder.Message);
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
        builder.Append(reminder.Target, ", ", ReminderFromSpace, creator, " (");
        builder.Advance(TimeSpanFormatter.Format(timeSinceReminderCreation, builder.FreeBufferSpan));
        builder.Append(" ago)");

        if (!string.IsNullOrWhiteSpace(reminder.Message))
        {
            builder.Append(": ", reminder.Message);
        }

        Reminders.Remove(reminder.Id);
        await SendAsync(reminder.Channel, builder.ToString());
    }
}
