using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
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
                emotePattern = new(new(messageExtension.Split[1].Span), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return;
            }
        }

        using PoolBufferList<string> emotesList = await _twitchBot.GetAllEmoteNames(ChatMessage.ChannelId);
        if (emotesList.Count == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoThirdPartyEmotesEnabledInThisChannel);
            return;
        }

        Memory<string> emotes = emotesList.AsMemory();
        if (emotePattern is not null)
        {
            WhereRegexIsMatch(ref emotes, emotePattern);
        }

        if (emotes.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereIsNoEmoteMatchingYourProvidedPattern);
            return;
        }

        Response.Append(ChatMessage.Username, ", [ ", emotes.Span.Random(), " ", emotes.Span.Random(), " ", emotes.Span.Random(), " ] (");
        Response.Append(emotes.Length);
        Response.Append(" emote", emotes.Length > 1 ? "s" : string.Empty, ")");
    }

    private static void WhereRegexIsMatch(ref Memory<string> emotes, Regex regex)
    {
        int resultLength = 0;
        Span<string> emoteSpan = emotes.Span;
        for (int i = 0; i < emotes.Length; i++)
        {
            string emote = emoteSpan[i];
            if (regex.IsMatch(emote))
            {
                emoteSpan[resultLength++] = emote;
            }
        }

        emotes = emotes[..resultLength];
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
