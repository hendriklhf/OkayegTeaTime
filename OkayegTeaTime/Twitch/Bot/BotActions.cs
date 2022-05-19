using System.Text;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Bot;

public static class BotActions
{
    public static void SendComingBack(this TwitchBot twitchBot, TwitchChatMessage chatMessage)
    {
        User? user = DbControl.Users[chatMessage.UserId];
        if (user is null)
        {
            return;
        }

        AfkCommand cmd = twitchBot.CommandController[user.AfkType];
        string? afkMessage = new AfkMessage(user.Id, cmd).ComingBack;
        if (afkMessage is null)
        {
            return;
        }

        twitchBot.Send(chatMessage.Channel, afkMessage);
    }

    public static void SendReminder(this TwitchBot twitchBot, string channel, IEnumerable<Reminder> reminders)
    {
        Reminder[] rmndrs = reminders.ToArray();
        if (rmndrs.Length == 0)
        {
            return;
        }

        string creator = rmndrs[0].Creator == rmndrs[0].Target ? "yourself" : rmndrs[0].Creator;
        string message = $"{rmndrs[0].Target}, reminder from {creator} ({TimeHelper.GetUnixDifference(rmndrs[0].Time)} ago)";
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

        rmndrs.ForEach(r => r.HasBeenSent = true);
        twitchBot.Send(channel, builder.ToString());
    }

    public static void SendTimedReminder(this TwitchBot twitchBot, Reminder reminder)
    {
        string creator = reminder.Target == reminder.Creator ? "yourself" : reminder.Creator;
        string message = $"{reminder.Target}, reminder from {creator} ({TimeHelper.GetUnixDifference(reminder.Time)} ago)";
        if (!string.IsNullOrEmpty(reminder.Message))
        {
            message += $": {reminder.Message}";
        }

        reminder.HasBeenSent = true;
        twitchBot.Send(reminder.Channel, message);
    }
}
