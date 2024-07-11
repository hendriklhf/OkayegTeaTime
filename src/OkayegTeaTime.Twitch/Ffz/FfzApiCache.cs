using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using HLE.Collections.Concurrent;
using OkayegTeaTime.Twitch.Ffz.Models;

namespace OkayegTeaTime.Twitch.Ffz;

public sealed class FfzApiCache(CacheOptions options) : IEquatable<FfzApiCache>
{
    public CacheOptions Options { get; set; } = options;

    private readonly ConcurrentDoubleDictionary<long, int, CacheEntry<ImmutableArray<Emote>>> _channelEmotesCache = new();
    private CacheEntry<ImmutableArray<Emote>> _globalEmotesCache = CacheEntry<ImmutableArray<Emote>>.Empty;

    public void AddChannelEmotes(long channelId, ReadOnlySpan<char> channelName, ImmutableArray<Emote> emotes)
    {
        int channelNameHash = string.GetHashCode(channelName, StringComparison.OrdinalIgnoreCase);
        _channelEmotesCache.AddOrSet(channelId, channelNameHash, new(emotes));
    }

    public void AddGlobalEmotes(ImmutableArray<Emote> emotes) => _globalEmotesCache = new(emotes);

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
        if (_channelEmotesCache.TryGetByPrimaryKey(channelId, out CacheEntry<ImmutableArray<Emote>> emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public bool TryGetChannelEmotes(ReadOnlySpan<char> channelName, out ImmutableArray<Emote> emotes)
    {
        int channelNameHash = string.GetHashCode(channelName, StringComparison.OrdinalIgnoreCase);
        if (_channelEmotesCache.TryGetBySecondaryKey(channelNameHash, out CacheEntry<ImmutableArray<Emote>> emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public bool Equals(FfzApiCache? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is FfzApiCache other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(FfzApiCache? left, FfzApiCache? right) => Equals(left, right);

    public static bool operator !=(FfzApiCache? left, FfzApiCache? right) => !(left == right);
}
