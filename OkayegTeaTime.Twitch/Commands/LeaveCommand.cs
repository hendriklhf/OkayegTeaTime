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
public readonly struct LeaveCommand : IChatCommand<LeaveCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public LeaveCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out LeaveCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s#?\w{3,25}");
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

            bool success = await _twitchBot.LeaveChannel(channel);
            Response.Append(ChatMessage.Username, ", ", success ? "successfully left" : "failed to leave", " #", channel);
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
