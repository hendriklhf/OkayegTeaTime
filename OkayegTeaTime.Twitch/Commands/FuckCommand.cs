using System;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Fuck)]
public readonly ref struct FuckCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref PoolBufferStringBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public FuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            _response.Append(Emoji.PointRight, " ", Emoji.OkHand, ChatMessage.Username, " fucked ", messageExtension.Split[1]);
            if (messageExtension.Split.Length > 2)
            {
                _response.Append(" ", messageExtension.Split[2]);
            }
        }
    }
}
