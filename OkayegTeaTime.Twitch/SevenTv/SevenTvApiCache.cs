using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using HLE.Collections;
using OkayegTeaTime.Twitch.SevenTv.Models;

namespace OkayegTeaTime.Twitch.SevenTv;

public sealed class SevenTvApiCache(CacheOptions options) : IEquatable<SevenTvApiCache>
{
    public CacheOptions Options { get; set; } = options;

    private CacheEntry<ImmutableArray<Emote>> _globalEmotesCache = CacheEntry<ImmutableArray<Emote>>.Empty;
    private readonly ConcurrentDictionary<long, CacheEntry<ImmutableArray<Emote>>> _channelEmotesCache = new();

    public void AddGlobalEmotes(ImmutableArray<Emote> emotes) => _globalEmotesCache = new(emotes);

    public void AddChannelEmotes(long channelId, ImmutableArray<Emote> emotes) => _channelEmotesCache.AddOrSet(channelId, new(emotes));

    public bool TryGetGlobalEmotes(out ImmutableArray<Emote> emotes)
    {
        if (_globalEmotesCache.IsValid(Options.GlobalEmotesCacheTime))
        {
            emotes = _globalEmotesCache.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public bool TryGetChannelEmotes(long channelId, out ImmutableArray<Emote> emotes)
    {
        if (_channelEmotesCache.TryGetValue(channelId, out CacheEntry<ImmutableArray<Emote>> emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public bool Equals(SevenTvApiCache? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is SevenTvApiCache other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(SevenTvApiCache? left, SevenTvApiCache? right) => Equals(left, right);

    public static bool operator !=(SevenTvApiCache? left, SevenTvApiCache? right) => !(left == right);
}
