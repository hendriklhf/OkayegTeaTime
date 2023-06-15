using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Memory;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Slots, typeof(SlotsCommand))]
public readonly struct SlotsCommand : IChatCommand<SlotsCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private static readonly RegexPool _emotePatternPool = new();

    public SlotsCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SlotsCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex? emotePattern = null;
        Regex pattern = _twitchBot.RegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                if (!_emotePatternPool.TryGet(messageExtension.Split[1].Span, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1), out emotePattern))
                {
                    emotePattern = new(new(messageExtension.Split[1].Span), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    _emotePatternPool.Add(emotePattern);
                }
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return;
            }
        }

        string[] allEmotes = await _twitchBot.EmoteService.GetAllEmoteNamesAsync(ChatMessage.ChannelId);
        if (allEmotes.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoThirdPartyEmotesEnabledInThisChannel);
            return;
        }

        using RentedArray<string> matchingEmotesBuffer = new(allEmotes.Length);
        ReadOnlyMemory<string> matchingEmotes = allEmotes;
        if (emotePattern is not null)
        {
            int matchingEmoteCount = GetMatchingEmotes(allEmotes, matchingEmotesBuffer, emotePattern);
            matchingEmotes = matchingEmotesBuffer.Memory[..matchingEmoteCount];
        }

        if (matchingEmotes.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoEmoteMatchingYourProvidedPattern);
            return;
        }

        Response.Append(ChatMessage.Username, ", [ ", matchingEmotes.Span.Random(), " ", matchingEmotes.Span.Random(), " ", matchingEmotes.Span.Random(), " ] (");
        Response.Append(matchingEmotes.Length);
        Response.Append(" emote", matchingEmotes.Length > 1 ? "s" : string.Empty, ")");
    }

    private static int GetMatchingEmotes(ReadOnlySpan<string> allEmotes, Span<string> matchingEmotes, Regex regex)
    {
        int matchingEmoteCount = 0;
        for (int i = 0; i < allEmotes.Length; i++)
        {
            string emote = allEmotes[i];
            if (regex.IsMatch(emote))
            {
                matchingEmotes[matchingEmoteCount++] = emote;
            }
        }

        return matchingEmoteCount;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
