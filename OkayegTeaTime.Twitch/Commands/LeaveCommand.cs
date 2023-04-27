using System;
using System.Text.RegularExpressions;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using StringHelper = OkayegTeaTime.Utils.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Leave)]
public readonly ref struct LeaveCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref PoolBufferStringBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public LeaveCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s#?\w{3,25}");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            if (!messageExtension.IsBotModerator)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOfTheBot);
                return;
            }

            string channel = new(messageExtension.LowerSplit[1]);
            bool isValidChannel = StringHelper.FormatChannel(ref channel);
            if (!isValidChannel)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.GivenChannelIsInvalid);
                return;
            }

            bool isJoined = _twitchBot.Channels[channel] is not null;
            if (!isJoined)
            {
                _response.Append(ChatMessage.Username, ", ", "the bot is not connected to #", channel);
                return;
            }

            bool success = _twitchBot.LeaveChannel(channel);
            _response.Append(ChatMessage.Username, ", ", success ? "successfully left" : "failed to leave", " #", channel);
        }
    }
}
