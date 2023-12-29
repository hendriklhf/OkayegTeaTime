using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Rafk, typeof(RafkCommand))]
public readonly struct RafkCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<RafkCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out RafkCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public ValueTask HandleAsync()
    {
        User? user = _twitchBot.Users.Get(ChatMessage.UserId, ChatMessage.Username);
        if (user is null)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.CantResumeYourAfkStatusBecauseYouNeverWentAfkBefore);
            return ValueTask.CompletedTask;
        }

        user.IsAfk = true;

        int afkMessageLength = _twitchBot.AfkMessageBuilder.BuildResumingMessage(ChatMessage.Username, user.AfkType, Response.FreeBufferSpan);
        Response.Advance(afkMessageLength);
        return ValueTask.CompletedTask;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(RafkCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is RafkCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(RafkCommand left, RafkCommand right) => left.Equals(right);

    public static bool operator !=(RafkCommand left, RafkCommand right) => !left.Equals(right);
}
