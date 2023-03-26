using System;
using System.Buffers;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Memory;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Slots)]
public readonly ref struct SlotsCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public SlotsCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex? emotePattern = null;
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                emotePattern = new(new(messageExtension.Split[1]), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return;
            }
        }

        int emoteCount = _twitchBot.EmoteController.GetEmoteCount(ChatMessage.ChannelId);
        using RentedArray<string> emoteBuffer = ArrayPool<string>.Shared.Rent(emoteCount);
        _twitchBot.EmoteController.GetAllEmoteNames(ChatMessage.ChannelId, emoteBuffer);
        Span<string> emotes = emoteBuffer[..emoteCount];

        if (emotes.Length == 0)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoThirdPartyEmotesEnabledInThisChannel);
            return;
        }

        if (emotePattern is not null)
        {
            WhereRegexIsMatch(ref emotes, emotePattern);
        }

        if (emotes.Length == 0)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoEmoteMatchingYourProvidedPattern);
            return;
        }

        _response.Append(ChatMessage.Username, ", [ ", emotes.Random(), " ", emotes.Random(), " ", emotes.Random(), " ] (");
        _response.Append(emotes.Length);
        _response.Append(" emote", emotes.Length > 1 ? "s" : string.Empty, ")");
    }

    private static void WhereRegexIsMatch(ref Span<string> emotes, Regex regex)
    {
        int resultLength = 0;
        for (int i = 0; i < emotes.Length; i++)
        {
            string emote = emotes[i];
            if (regex.IsMatch(emote))
            {
                emotes[resultLength++] = emote;
            }
        }

        emotes = emotes[..resultLength];
    }
}
