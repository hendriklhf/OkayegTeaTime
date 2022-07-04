using System.Collections.Generic;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            string username = ChatMessage.LowerSplit[2];
            long? userId = _twitchBot.TwitchApi.GetUserId(username);
            if (!userId.HasValue)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            User? user = DbControl.Users.GetUser(userId.Value, username);
            if (user is null)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            if (user.IsAfk)
            {
                AfkCommand cmd = _twitchBot.CommandController[user.AfkType];
                Response += new AfkMessage(userId.Value, cmd).GoingAway!;
                string? message = user.AfkMessage;
                if (!string.IsNullOrEmpty(message))
                {
                    Response += $": {message}";
                }

                Response += $" ({TimeHelper.GetUnixDifference(user.AfkTime)} ago)";
            }
            else
            {
                Response += $"{username} is not afk";
            }

            return;
        }

        pattern = PatternCreator.Create(Alias, Prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            int id = ChatMessage.Split[2].ToInt();
            Reminder? reminder = DbControl.Reminders[id];
            if (reminder is null)
            {
                Response += PredefinedMessages.ReminderNotFoundMessage;
                return;
            }

            List<string> responses = new()
            {
                $"From: {reminder.Creator} || To: {reminder.Target}",
                $"Set: {TimeHelper.GetUnixDifference(reminder.Time)} ago"
            };

            if (reminder.ToTime > 0)
            {
                responses.Add($"Fires in: {TimeHelper.GetUnixDifference(reminder.ToTime)}");
            }

            if (!string.IsNullOrEmpty(reminder.Message))
            {
                responses.Add($"Message: {reminder.Message}");
            }

            Response = responses.JoinToString(" || ");
        }
    }
}
