using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Check, typeof(CheckCommand))]
public readonly struct CheckCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<CheckCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CheckCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\safk\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            await CheckAfkStatus();
            return;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            CheckReminder();
        }
    }

    private void CheckReminder()
    {
        Response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        int reminderId = int.Parse(messageExtension.Split[2].Span);
        Reminder? reminder = _twitchBot.Reminders[reminderId];
        if (reminder is null)
        {
            Response.Append(Messages.CouldNotFindAnyMatchingReminder);
            return;
        }

        TimeSpan timeSinceReminderCreation = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(reminder.Time);
        using PooledList<string> reminderProps = new(4)
        {
            $"From: {reminder.Creator} || To: {reminder.Target}",
            $"Set: {TimeSpanFormatter.Format(timeSinceReminderCreation)} ago"
        };

        if (reminder.ToTime > 0)
        {
            timeSinceReminderCreation = DateTimeOffset.FromUnixTimeMilliseconds(reminder.ToTime) - DateTime.UtcNow;
            reminderProps.Add($"Fires in: {TimeSpanFormatter.Format(timeSinceReminderCreation)}");
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

        int afkMessageLength = _twitchBot.AfkMessageBuilder.BuildGoingAwayMessage(username, user.AfkType, Response.FreeBufferSpan);
        Response.Advance(afkMessageLength);

        string? afkMessage = user.AfkMessage;
        if (!string.IsNullOrWhiteSpace(afkMessage))
        {
            Response.Append(": ", afkMessage);
        }

        TimeSpan timeSinceBeingAfk = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
        Response.Append(" (");
        Response.Advance(TimeSpanFormatter.Format(timeSinceBeingAfk, Response.FreeBufferSpan));
        Response.Append(" ago)");
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(CheckCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is CheckCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(CheckCommand left, CheckCommand right) => left.Equals(right);

    public static bool operator !=(CheckCommand left, CheckCommand right) => !left.Equals(right);
}
