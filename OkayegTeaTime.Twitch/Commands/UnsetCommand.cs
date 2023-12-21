using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Channel = OkayegTeaTime.Database.Models.Channel;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Unset, typeof(UnsetCommand))]
public readonly struct UnsetCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<UnsetCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out UnsetCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask Handle()
    {
        ReadOnlySpan<char> alias = _alias.Span;
        ReadOnlySpan<char> prefix = _prefix.Span;
        Regex pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\sprefix");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetPrefix();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\sreminder\s\d+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetReminder();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\semote");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetEmote();
            return ValueTask.CompletedTask;
        }

        pattern = _twitchBot.MessageRegexCreator.Create(alias, prefix, @"\slocation");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            UnsetLocation();
        }

        return ValueTask.CompletedTask;
    }

    private void UnsetPrefix()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.AnErrorOccurredWhileTryingToSetThePrefix);
            return;
        }

        channel.Prefix = null;
        Response.Append(ChatMessage.Username, ", ", Messages.ThePrefixHasBeenUnset);
    }

    private void UnsetReminder()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        Response.Append(ChatMessage.Username, ", ");
        int reminderId = int.Parse(messageExtension.Split[2].Span);
        bool removed = _twitchBot.Reminders.Remove(ChatMessage.UserId, ChatMessage.Username, reminderId);
        Response.Append(removed ? Messages.TheReminderHasBeenUnset : Messages.TheReminderCouldntBeUnset);
    }

    private void UnsetEmote()
    {
        Response.Append(ChatMessage.Username, ", ");
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!ChatMessage.IsModerator && !messageExtension.IsBroadcaster)
        {
            Response.Append(Messages.YouArentAModeratorOrTheBroadcaster);
            return;
        }

        Channel? channel = _twitchBot.Channels[ChatMessage.ChannelId];
        if (channel is null)
        {
            Response.Append(Messages.AnErrorOccurredWhileTryingToSetTheEmote);
            return;
        }

        channel.Emote = null;
        Response.Append(Messages.TheEmoteHasBeenUnset);
    }

    private void UnsetLocation()
    {
        User? user = _twitchBot.Users[ChatMessage.UserId];
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouHaventSetYourLocationYet);
            return;
        }

        user.Location = null;
        Response.Append(ChatMessage.Username, ", ", Messages.YourLocationHasBeenUnset);
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(UnsetCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is UnsetCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(UnsetCommand left, UnsetCommand right) => left.Equals(right);

    public static bool operator !=(UnsetCommand left, UnsetCommand right) => !left.Equals(right);
}
