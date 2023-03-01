using System;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Check)]
public readonly ref struct CheckCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;

    private readonly ref MessageBuilder _response;
    private readonly string? _prefix;
    private readonly string _alias;

    public CheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            CheckAfkStatus();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            CheckReminder();
        }
    }

    private void CheckReminder()
    {
        _response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        int id = int.Parse(messageExtension.Split[2]);
        Reminder? reminder = _twitchBot.Reminders[id];
        if (reminder is null)
        {
            _response.Append(Messages.CouldNotFindAnyMatchingReminder);
            return;
        }

        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
        using PoolBufferList<string> reminderProps = new(4)
        {
            $"From: {reminder.Creator} || To: {reminder.Target}",
            $"Set: {timeSinceReminderCreation.Format()} ago"
        };

        if (reminder.ToTime > 0)
        {
            timeSinceReminderCreation = DateTimeOffset.FromUnixTimeMilliseconds(reminder.ToTime) - DateTime.UtcNow;
            reminderProps.Add($"Fires in: {timeSinceReminderCreation.Format()}");
        }

        if (!string.IsNullOrWhiteSpace(reminder.Message))
        {
            reminderProps.Add($"Message: {reminder.Message}");
        }

        Span<char> joinBuffer = stackalloc char[250];
        int bufferLength = StringHelper.Join(reminderProps.AsSpan(), " || ", joinBuffer);
        _response.Append(joinBuffer[..bufferLength]);
    }

    private void CheckAfkStatus()
    {
        _response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        string username = new(messageExtension.LowerSplit[2]);
        long userId = _twitchBot.TwitchApi.GetUserId(username);
        if (userId == -1)
        {
            _response.Append(Messages.CouldNotFindAnyMatchingUser);
            return;
        }

        User? user = _twitchBot.Users.Get(userId, username);
        if (user is null)
        {
            _response.Append(Messages.CouldNotFindAnyMatchingUser);
            return;
        }

        if (!user.IsAfk)
        {
            _response.Append(username, " is not afk");
            return;
        }

        AfkCommand afkCommand = _twitchBot.CommandController[user.AfkType];
        _response.Append(new AfkMessage(user, afkCommand).GoingAway);
        string? message = user.AfkMessage;
        if (!string.IsNullOrWhiteSpace(message))
        {
            _response.Append(": ", message);
        }

        TimeSpan timeSinceBeingAfk = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
        Span<char> spanChars = stackalloc char[100];
        int spanCharsLength = timeSinceBeingAfk.Format(spanChars);
        _response.Append(" (", spanChars[..spanCharsLength], " ago)");
    }
}
