using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HLE;
using HLE.Collections;
using HLE.Strings;
using TwitchEmote = OkayegTeaTime.Twitch.Helix.Models.Emote;
using TwitchChannelEmote = OkayegTeaTime.Twitch.Helix.Models.ChannelEmote;
using FfzEmote = OkayegTeaTime.Twitch.Ffz.Models.Emote;
using SevenTvEmote = OkayegTeaTime.Twitch.SevenTv.Models.Emote;
using BttvEmote = OkayegTeaTime.Twitch.Bttv.Models.Emote;

namespace OkayegTeaTime.Twitch.Services;

public sealed class EmoteService(TwitchBot twitchBot) : IEquatable<EmoteService>
{
    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ConcurrentDictionary<long, CacheEntry<StringArray>> _emoteNamesCache = new();

    private static readonly TimeSpan s_emoteNamesCacheTime = TimeSpan.FromHours(1);

    public async ValueTask<string> GetBestEmoteAsync(long channelId, string fallback, params string[] keywords)
    {
        StringArray emoteNames = await GetAllEmoteNamesAsync(channelId);
        using PooledList<string> bestEmotes = new(32);
        foreach (string keyword in keywords)
        {
            for (int i = 0; i < emoteNames.Length; i++)
            {
                AddEmoteIfContainsKeyword(emoteNames, i, keyword, bestEmotes);
            }
        }

        return bestEmotes.Count == 0 ? fallback : Random.Shared.GetItem(bestEmotes.AsSpan());
    }

    private static void AddEmoteIfContainsKeyword(StringArray emoteNames, int i, string keyword, PooledList<string> bestEmotes)
    {
        ReadOnlySpan<char> emoteName = emoteNames.GetChars(i);
        if (emoteName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            bestEmotes.Add(emoteNames[i]);
        }
    }

    public async ValueTask<StringArray> GetAllEmoteNamesAsync(long channelId)
    {
        if (_emoteNamesCache.TryGetValue(channelId, out CacheEntry<StringArray> emoteNamesEntry) && emoteNamesEntry.IsValid(s_emoteNamesCacheTime))
        {
            return emoteNamesEntry.Value;
        }

        ImmutableArray<TwitchEmote> twitchGlobalEmotes = await _twitchBot.TwitchApi.GetGlobalEmotesAsync();
        ImmutableArray<TwitchChannelEmote> twitchChannelEmotes = await _twitchBot.TwitchApi.GetChannelEmotesAsync(channelId);
        ImmutableArray<FfzEmote> ffzGlobalEmotes = await _twitchBot.FfzApi.GetGlobalEmotesAsync();
        ImmutableArray<FfzEmote> ffzChannelEmotes = await _twitchBot.FfzApi.GetChannelEmotesAsync(channelId);
        ImmutableArray<BttvEmote> bttvGlobalEmotes = await _twitchBot.BttvApi.GetGlobalEmotesAsync();
        ImmutableArray<BttvEmote> bttvChannelEmotes = await _twitchBot.BttvApi.GetChannelEmotesAsync(channelId);
        ImmutableArray<SevenTvEmote> sevenTvGlobalEmotes = await _twitchBot.SevenTvApi.GetGlobalEmotesAsync();
        ImmutableArray<SevenTvEmote> sevenTvChannelEmotes = await _twitchBot.SevenTvApi.GetChannelEmotesAsync(channelId);

        int totalEmoteCount = twitchGlobalEmotes.Length + twitchChannelEmotes.Length + ffzGlobalEmotes.Length + ffzChannelEmotes.Length +
                              bttvGlobalEmotes.Length + bttvChannelEmotes.Length + sevenTvGlobalEmotes.Length + sevenTvChannelEmotes.Length;

        StringArray emoteNames = new(totalEmoteCount);
        int emoteNameCount = 0;

        foreach (TwitchEmote emote in twitchGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (TwitchChannelEmote emote in twitchChannelEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (FfzEmote emote in ffzGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (FfzEmote emote in ffzChannelEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (BttvEmote emote in bttvGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (BttvEmote emote in bttvChannelEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (SevenTvEmote emote in sevenTvGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (SevenTvEmote emote in sevenTvChannelEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        _emoteNamesCache.AddOrSet(channelId, new(emoteNames));
        return emoteNames;
    }

    public bool Equals(EmoteService? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is EmoteService other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(EmoteService? left, EmoteService? right) => Equals(left, right);

    public static bool operator !=(EmoteService? left, EmoteService? right) => !(left == right);
}
