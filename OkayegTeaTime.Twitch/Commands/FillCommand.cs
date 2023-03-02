using System;
using System.Text.RegularExpressions;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Fill)]
public readonly ref struct FillCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public FillCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string emote = _twitchBot.Channels[ChatMessage.ChannelId]?.Emote ?? AppSettings.DefaultEmote;
            int maxLength = AppSettings.MaxMessageLength - (emote.Length + 1);
            int maxSplitIndex = messageExtension.Split.Length - 1;
            ReadOnlySpan<char> nextMessagePart = messageExtension.Split[Random.Int(1, maxSplitIndex)];
            for (int currentMessageLength = 0; currentMessageLength + nextMessagePart.Length + 1 < maxLength; currentMessageLength += nextMessagePart.Length + 1)
            {
                _response.Append(nextMessagePart, " ");
                nextMessagePart = messageExtension.Split[Random.Int(1, maxSplitIndex)];
            }
        }
    }
}
