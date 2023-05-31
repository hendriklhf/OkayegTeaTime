using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Check, typeof(CheckCommand))]
public readonly struct CheckCommand : IChatCommand<CheckCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;

    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public CheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CheckCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await CheckAfkStatus();
            return;
        }

        pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            CheckReminder();
        }
    }

    private void CheckReminder()
    {
        Response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        int id = int.Parse(messageExtension.Split[2].Span);
        Reminder? reminder = _twitchBot.Reminders[id];
        if (reminder is null)
        {
            Response.Append(Messages.CouldNotFindAnyMatchingReminder);
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

        int joinLength = StringHelper.Join(reminderProps.AsSpan(), " || ", Response.FreeBufferSpan);
        Response.Advance(joinLength);
    }

    private async ValueTask CheckAfkStatus()
    {
        Response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        string username = new(messageExtension.LowerSplit[2].Span);

        var twitchUser = await _twitchBot.TwitchApi.GetUserAsync(username);
        if (twitchUser is null)
        {
            Response.Append(Messages.CouldNotFindAnyMatchingUser);
            return;
        }

        User? user = _twitchBot.Users.Get(twitchUser.Id, username);
        if (user is null)
        {
            Response.Append(Messages.CouldNotFindAnyMatchingUser);
            return;
        }

        if (!user.IsAfk)
        {
            Response.Append(username, " is not afk");
            return;
        }

        AfkCommand afkCommand = _twitchBot.CommandController[user.AfkType];
        Response.Append(new AfkMessage(user, afkCommand).GoingAway);
        string? message = user.AfkMessage;
        if (!string.IsNullOrWhiteSpace(message))
        {
            Response.Append(": ", message);
        }

        TimeSpan timeSinceBeingAfk = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
        Response.Append(" (");
        Response.Advance(timeSinceBeingAfk.Format(Response.FreeBufferSpan));
        Response.Append(" ago)");
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
