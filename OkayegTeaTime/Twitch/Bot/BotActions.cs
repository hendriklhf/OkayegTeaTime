using System.Text;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Commands.AfkCommandClasses;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Bot;

public static class BotActions
{
    public static void SendComingBack(this TwitchBot twitchBot, TwitchChatMessage chatMessage)
    {
        string? afkMessage = new AfkMessage(chatMessage.UserId).ComingBack;
        if (afkMessage is null)
        {
            return;
        }

        twitchBot.Send(chatMessage.Channel, afkMessage);
    }

    public static void SendReminder(this TwitchBot twitchBot, TwitchChatMessage chatMessage, IEnumerable<Reminder> reminders)
    {
        Reminder[] rmndrs = reminders.ToArray();
        string creator = rmndrs[0].Creator == rmndrs[0].Target ? "yourself" : rmndrs[0].Creator;
        string message = $"{chatMessage.Username}, reminder from {creator} ({TimeHelper.GetUnixDifference(rmndrs[0].Time)} ago)";
        StringBuilder builder = new(message);
        if (rmndrs[0].Message?.Length > 0)
        {
            builder.Append($": {rmndrs[0].Message}");
        }

        if (rmndrs.Length > 1)
        {
            rmndrs.Skip(1).ForEach(r =>
            {
                string c = r.Creator == r.Target ? "yourself" : r.Creator;
                builder.Append($" || {c} ({TimeHelper.GetUnixDifference(r.Time)} ago)");
                if (r.Message?.Length > 0)
                {
                    builder.Append($": {r.Message}");
                }
            });
        }
        twitchBot.Send(chatMessage.Channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string creator = reminder.Target == reminder.Creator ? "yourself" : reminder.Creator;
        string message = $"{reminder.Target}, reminder from {creator} ({TimeHelper.GetUnixDifference(reminder.Time)} ago)";
        if (!string.IsNullOrEmpty(reminder.Message))
        {
            message += $": {reminder.Message}";
        }
        twitchBot.Send(reminder.Channel, message);
    }
}
