using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Leave, typeof(LeaveCommand))]
public readonly struct LeaveCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<LeaveCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out LeaveCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!messageExtension.IsBotModerator)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = StringPool.Shared.GetOrAdd(messageExtension.LowerSplit[1].Span);
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (!isJoined)
            {
                Response.Append(ChatMessage.Username, ", ", "the bot is not connected to #", channel);
                return;
            }

            bool success = await _twitchBot.LeaveChannelAsync(channel);
            Response.Append(ChatMessage.Username, ", ", success ? "successfully left" : "failed to leave", " #", channel);
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }

    public bool Equals(LeaveCommand other)
    {
        return _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);
    }

    public override bool Equals(object? obj)
    {
        return obj is LeaveCommand other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);
    }

    public static bool operator ==(LeaveCommand left, LeaveCommand right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(LeaveCommand left, LeaveCommand right)
    {
        return !left.Equals(right);
    }
}
