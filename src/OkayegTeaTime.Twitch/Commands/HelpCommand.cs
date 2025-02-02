using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<HelpCommand>(CommandType.Help)]
public readonly struct HelpCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<HelpCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out HelpCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    [SuppressMessage("Roslynator", "RCS1229:Use async/await when necessary", Justification = "ChatMessageExtension can be disposed before the Task is awaited")]
    public ValueTask HandleAsync()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        ReadOnlySpan<char> username = messageExtension.Split.Length > 1 ? messageExtension.LowerSplit[1].Span : ChatMessage.Username.AsSpan();
        Response.Append($"{Emoji.PointRight} {username}, here you can find a list of commands and the repository: {GlobalSettings.Settings.RepositoryUrl}");
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(HelpCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is HelpCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(HelpCommand left, HelpCommand right) => left.Equals(right);

    public static bool operator !=(HelpCommand left, HelpCommand right) => !left.Equals(right);
}
