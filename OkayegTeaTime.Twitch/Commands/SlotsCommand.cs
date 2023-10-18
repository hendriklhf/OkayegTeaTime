using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Marshalling;
using HLE.Memory;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Slots, typeof(SlotsCommand))]
public readonly struct SlotsCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SlotsCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SlotsCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask Handle()
    {
        Regex? emotePattern = null;
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            try
            {
                using ChatMessageExtension messageExtension = new(ChatMessage);
                if (!RegexPool.Shared.TryGet(messageExtension.Split[1].Span, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1), out emotePattern))
                {
                    emotePattern = new(new(messageExtension.Split[1].Span), RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                    RegexPool.Shared.Add(emotePattern);
                }
            }
            catch (ArgumentException)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheGivenPatternIsInvalid);
                return;
            }
        }

        StringArray allEmotes = await _twitchBot.EmoteService.GetAllEmoteNamesAsync(ChatMessage.ChannelId);
        if (allEmotes.Length == 0)
        {
            Response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoThirdPartyEmotesEnabledInThisChannel);
            return;
        }

        using RentedArray<string> matchingEmotesBuffer = new(allEmotes.Length);
        ReadOnlyMemory<string> matchingEmotes = StringArrayMarshal.GetStrings(allEmotes);
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

    private static int GetMatchingEmotes(StringArray allEmotes, Span<string> matchingEmotes, Regex regex)
    {
        int matchingEmoteCount = 0;
        for (int i = 0; i < allEmotes.Length; i++)
        {
            ReadOnlySpan<char> emote = allEmotes.GetChars(i);
            if (regex.IsMatch(emote))
            {
                matchingEmotes[matchingEmoteCount++] = allEmotes[i];
            }
        }

        return matchingEmoteCount;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(SlotsCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is SlotsCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(SlotsCommand left, SlotsCommand right) => left.Equals(right);

    public static bool operator !=(SlotsCommand left, SlotsCommand right) => !left.Equals(right);
}
