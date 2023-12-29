using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Id, typeof(IdCommand))]
public readonly struct IdCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<IdCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out IdCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Response.Append(ChatMessage.Username, ", ");
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\w+");
        long userId;
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlyMemory<char> username = messageExtension.LowerSplit[1];
            userId = await GetUserIdAsync(username);
            if (userId < 0)
            {
                Response.Append(Messages.TwitchUserDoesntExist);
                return;
            }
        }
        else
        {
            userId = ChatMessage.UserId;
            Response.Append("your id: ");
        }

        Response.Append(userId);
    }

    private async ValueTask<long> GetUserIdAsync(ReadOnlyMemory<char> username)
    {
        if (username.Span.SequenceEqual(ChatMessage.Username))
        {
            return ChatMessage.UserId;
        }

        User? twitchUser = await _twitchBot.TwitchApi.GetUserAsync(username);
        if (twitchUser is null)
        {
            return -1;
        }

        return twitchUser.Id;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(IdCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is IdCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(IdCommand left, IdCommand right) => left.Equals(right);

    public static bool operator !=(IdCommand left, IdCommand right) => !left.Equals(right);
}
