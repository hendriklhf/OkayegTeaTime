using System;
using System.Text.RegularExpressions;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Id)]
public readonly ref struct IdCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref PoolBufferStringBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public IdCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        _response.Append(ChatMessage.Username, ", ");
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+");
        long userId;
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            ReadOnlySpan<char> username = messageExtension.LowerSplit[1];
            userId = _twitchBot.TwitchApi.GetUserId(new(username));
            if (userId == -1)
            {
                _response.Append(Messages.TwitchUserDoesntExist);
                return;
            }
        }
        else
        {
            userId = ChatMessage.UserId;
        }

        _response.Append(userId);
    }
}
