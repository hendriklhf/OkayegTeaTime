using System;
using System.Threading.Tasks;
using HLE;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Coinflip, typeof(CoinflipCommand))]
public readonly struct CoinflipCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<CoinflipCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out CoinflipCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        bool result = Random.Shared.NextBool();
        string answer = result ? "yes/heads" : "no/tails";
        Response.Append(ChatMessage.Username, ", ", answer, " ", Emoji.Coin);
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(CoinflipCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is CoinflipCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(CoinflipCommand left, CoinflipCommand right) => left.Equals(right);

    public static bool operator !=(CoinflipCommand left, CoinflipCommand right) => !left.Equals(right);
}
