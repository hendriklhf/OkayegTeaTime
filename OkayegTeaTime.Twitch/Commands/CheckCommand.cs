using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Check)]
public sealed class CheckCommand : Command
{
    public CheckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            string username = ChatMessage.LowerSplit[2];
            long userId = _twitchBot.TwitchApi.GetUserId(username);
            if (userId == -1)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            User? user = _twitchBot.Users.GetUser(userId, username);
            if (user is null)
            {
                Response += PredefinedMessages.UserNotFoundMessage;
                return;
            }

            if (user.IsAfk)
            {
                AfkCommand cmd = _twitchBot.CommandController[user.AfkType];
                Response += new AfkMessage(user, cmd).GoingAway;
                string? message = user.AfkMessage;
                if (!string.IsNullOrEmpty(message))
                {
                    Response += $": {message}";
                }

                TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
                Response += $" ({span.Format()} ago)";
            }
            else
            {
                Response += $"{username} is not afk";
            }

            return;
        }

        pattern = PatternCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            int id = int.Parse(ChatMessage.Split[2]);
            Reminder? reminder = _twitchBot.Reminders[id];
            if (reminder is null)
            {
                Response += PredefinedMessages.ReminderNotFoundMessage;
                return;
            }

            TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
            List<string> reminderProps = new()
            {
                $"From: {reminder.Creator} || To: {reminder.Target}",
                $"Set: {span.Format()} ago"
            };

            if (reminder.ToTime > 0)
            {
                span = DateTimeOffset.FromUnixTimeMilliseconds(reminder.ToTime) - DateTime.UtcNow;
                reminderProps.Add($"Fires in: {span.Format()}");
            }

            if (!string.IsNullOrEmpty(reminder.Message))
            {
                reminderProps.Add($"Message: {reminder.Message}");
            }

            Response = reminderProps.JoinToString(" || ");
        }
    }
}
