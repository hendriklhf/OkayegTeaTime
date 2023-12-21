using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public readonly struct InvalidateCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<InvalidateCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out InvalidateCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!messageExtension.IsBotModerator)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOfTheBot);
            return ValueTask.CompletedTask;
        }

        _twitchBot.InvalidateCaches();
        Response.Append(ChatMessage.Username, ", all database caches have been invalidated");
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(InvalidateCommand other) => _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is InvalidateCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(InvalidateCommand left, InvalidateCommand right) => left.Equals(right);

    public static bool operator !=(InvalidateCommand left, InvalidateCommand right) => !left.Equals(right);
}
