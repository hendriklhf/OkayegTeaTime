using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino7, typeof(Chatterino7Command))]
public readonly struct Chatterino7Command(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<Chatterino7Command>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private const string ResponseMessage = "Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases";

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out Chatterino7Command command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask Handle()
    {
        Response.Append(ChatMessage.Username, ", ", ResponseMessage);
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(Chatterino7Command other)
        => _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is Chatterino7Command other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(Chatterino7Command left, Chatterino7Command right) => left.Equals(right);

    public static bool operator !=(Chatterino7Command left, Chatterino7Command right) => !left.Equals(right);
}
