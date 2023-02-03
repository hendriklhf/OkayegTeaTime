using System;
using HLE;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch;

public static class BotActions
{
    private const string _yourself = "yourself";
    private const string _reminderFromSpace = "reminder from ";
    private const string _spaceParentheses = " (";
    private const string _spaceAgoParentheses = " ago)";
    private const string _colonSpace = ": ";
    private const string _spaceBarBarSpace = " || ";

    public static void SendComingBack(this TwitchBot twitchBot, long userId, string channel)
    {
        User? user = twitchBot.Users[userId];
        if (user is null)
        {
            return;
        }

        SendComingBack(twitchBot, user, channel);
    }

    public static void SendComingBack(this TwitchBot twitchBot, User user, string channel)
    {
        AfkCommand cmd = twitchBot.CommandController[user.AfkType];
        string afkMessage = new AfkMessage(user, cmd).ComingBack;
        twitchBot.Send(channel, afkMessage);
    }

    public static void SendReminder(this TwitchBot twitchBot, string channel, ReadOnlySpan<Reminder> reminders)
    {
        if (reminders.Length == 0)
        {
            return;
        }

        string creator = reminders[0].Creator == reminders[0].Target ? _yourself : reminders[0].Creator;
        TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminders[0].Time);
        Span<char> spanBuffer = stackalloc char[100];
        int spanLength = span.Format(spanBuffer);

        StringBuilder builder = stackalloc char[2048];
        builder.Append(reminders[0].Target, Commands.Messages.CommaSpace, _reminderFromSpace, creator, _spaceParentheses, spanBuffer[..spanLength], _spaceAgoParentheses);
        twitchBot.Reminders.Remove(reminders[0].Id);
        if (reminders[0].Message?.Length > 0)
        {
            builder.Append(_colonSpace, reminders[0].Message);
        }

        foreach (Reminder r in reminders[1..])
        {
            twitchBot.Reminders.Remove(r.Id);
            creator = r.Creator == r.Target ? _yourself : r.Creator;
            span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(r.Time);
            spanLength = span.Format(spanBuffer);
            builder.Append(_spaceBarBarSpace, creator, _spaceParentheses, spanBuffer[..spanLength], _spaceAgoParentheses);
            if (r.Message?.Length > 0)
            {
                builder.Append(_colonSpace, r.Message);
            }
        }

        twitchBot.Send(channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string creator = reminder.Target == reminder.Creator ? _yourself : reminder.Creator;
        TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
        Span<char> spanBuffer = stackalloc char[100];
        int spanLength = span.Format(spanBuffer);

        StringBuilder builder = stackalloc char[500];
        builder.Append(reminder.Target, Commands.Messages.CommaSpace, _reminderFromSpace, creator, _spaceParentheses, spanBuffer[..spanLength], _spaceAgoParentheses);
        if (!string.IsNullOrWhiteSpace(reminder.Message))
        {
            builder.Append(_colonSpace, reminder.Message);
        }

        twitchBot.Reminders.Remove(reminder.Id);
        twitchBot.Send(reminder.Channel, builder.ToString());
    }
}
