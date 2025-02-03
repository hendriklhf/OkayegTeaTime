using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE;
using HLE.Collections;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<SlotsCommand>(CommandType.Slots)]
public readonly struct SlotsCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<SlotsCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out SlotsCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        Regex? emotePattern = null;
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message.AsSpan()))
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
                Response.Append($"{ChatMessage.Username}, {Texts.TheGivenPatternIsInvalid}");
                return;
            }
        }

        ImmutableArray<string> allEmotes = await _twitchBot.EmoteService.GetAllEmoteNamesAsync(ChatMessage.ChannelId);
        if (allEmotes.Length == 0)
        {
            Response.Append($"{ChatMessage.Username}, {Texts.ThereAreNoThirdPartyEmotesEnabledInThisChannel}");
            return;
        }

        ReadOnlySpan<string> matchingEmotes = allEmotes.AsSpan();
        ValueList<string> matchingEmotesList = [];
        try
        {
            if (emotePattern is not null)
            {
                GetMatchingEmotes(allEmotes, emotePattern, ref matchingEmotesList);
                matchingEmotes = matchingEmotesList.AsSpan();
            }

            if (matchingEmotes.Length == 0)
            {
                Response.Append($"{ChatMessage.Username}, {Texts.ThereIsNoEmoteMatchingYourProvidedPattern}");
                return;
            }

            string firstRandomEmote = Random.Shared.GetItem(matchingEmotes);
            string secondRandomEmote = Random.Shared.GetItem(matchingEmotes);
            string thirdRandomEmote = Random.Shared.GetItem(matchingEmotes);

            Response.Append($"{ChatMessage.Username}, [ {firstRandomEmote} , {secondRandomEmote} , {thirdRandomEmote} ]");
            Response.Append($" ({matchingEmotes.Length} emote{(matchingEmotes.Length > 1 ? "s" : string.Empty)})");
        }
        finally
        {
            matchingEmotesList.Dispose();
        }
    }

    private static void GetMatchingEmotes(ImmutableArray<string> allEmotes, Regex regex, ref ValueList<string> matchingEmotes)
    {
        for (int i = 0; i < allEmotes.Length; i++)
        {
            ReadOnlySpan<char> emote = allEmotes[i];
            if (regex.IsMatch(emote))
            {
                matchingEmotes.Add(allEmotes[i]);
            }
        }
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
