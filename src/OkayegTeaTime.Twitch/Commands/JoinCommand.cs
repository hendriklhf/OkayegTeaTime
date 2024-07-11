using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Join, typeof(JoinCommand))]
public readonly struct JoinCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<JoinCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out JoinCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!messageExtension.IsBotModerator)
            {
                Response.Append(ChatMessage.Username, ", ", Texts.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = new(messageExtension.LowerSplit[1].Span);
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response.Append(ChatMessage.Username, ", ", Texts.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (isJoined)
            {
                Response.Append(ChatMessage.Username, ", ", "the bot is already connected to #", channel);
                return;
            }

            User? user = await _twitchBot.TwitchApi.GetUserAsync(channel);
            if (user is null)
            {
                Response.Append(ChatMessage.Username, ", ", Texts.GivenChannelDoesNotExist);
                return;
            }

            bool success = await _twitchBot.JoinChannelAsync(channel);
            Response.Append(ChatMessage.Username, ", ", success ? "successfully joined" : "failed to join", " #", channel);
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(JoinCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is JoinCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(JoinCommand left, JoinCommand right) => left.Equals(right);

    public static bool operator !=(JoinCommand left, JoinCommand right) => !left.Equals(right);
}
