using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Check)]
public readonly unsafe ref struct CheckCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public CheckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
            string username = ChatMessage.LowerSplit[2];
            long userId = _twitchBot.TwitchApi.GetUserId(username);
            if (userId == -1)
            {
                Response->Append(PredefinedMessages.CouldNotFindAnyMatchingUser);
                return;
            }

            User? user = _twitchBot.Users.Get(userId, username);
            if (user is null)
            {
                Response->Append(PredefinedMessages.CouldNotFindAnyMatchingUser);
                return;
            }

            if (user.IsAfk)
            {
                AfkCommand cmd = _twitchBot.CommandController[user.AfkType];
                Response->Append(new AfkMessage(user, cmd).GoingAway);
                string? message = user.AfkMessage;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Response->Append(": ", message);
                }

                TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
                Span<char> spanChars = stackalloc char[100];
                int spanCharsLength = span.Format(spanChars);
                Response->Append(" (", spanChars[..spanCharsLength], " ago)");
            }
            else
            {
                Response->Append(username, " is not afk");
            }

            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
            int id = int.Parse(ChatMessage.Split[2]);
            Reminder? reminder = _twitchBot.Reminders[id];
            if (reminder is null)
            {
                Response->Append(PredefinedMessages.CouldNotFindAnyMatchingReminder);
                return;
            }

            TimeSpan span = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
            // TODO: get rid of this list, unnecessarily allocates
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

            if (!string.IsNullOrWhiteSpace(reminder.Message))
            {
                reminderProps.Add($"Message: {reminder.Message}");
            }

            Response->Append(reminderProps.JoinToString(" || "));
        }
    }
}
