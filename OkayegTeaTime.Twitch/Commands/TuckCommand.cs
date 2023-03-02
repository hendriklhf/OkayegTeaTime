using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Tuck)]
public readonly ref struct TuckCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public TuckCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
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
            ReadOnlySpan<char> target = messageExtension.LowerSplit[1];
            _response.Append(Emoji.PointRight, " ", Emoji.Bed, " ", ChatMessage.Username);
            _response.Append(" tucked ", target, " to bed");
            ReadOnlySpan<char> emote = messageExtension.LowerSplit.Length > 2 ? messageExtension.Split[2] : string.Empty;
            if (emote.Length > 0)
            {
                _response.Append(" ", emote);
            }
        }
    }
}
