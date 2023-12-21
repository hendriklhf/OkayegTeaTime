using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using HLE.Collections;
using OkayegTeaTime.Twitch.Bttv.Models;

namespace OkayegTeaTime.Twitch.Bttv;

public sealed class BttvApiCache(CacheOptions options) : IEquatable<BttvApiCache>
{
    public CacheOptions Options { get; } = options;

    private readonly ConcurrentDictionary<long, CacheEntry<ImmutableArray<Emote>>> _channelEmoteCache = new();
    private CacheEntry<ImmutableArray<Emote>> _globalEmoteCache = CacheEntry<ImmutableArray<Emote>>.Empty;

    public void AddChannelEmotes(long channelId, ImmutableArray<Emote> emotes) => _channelEmoteCache.AddOrSet(channelId, new(emotes));

    public bool TryGetChannelEmotes(long channelId, out ImmutableArray<Emote> emotes)
    {
        if (_channelEmoteCache.TryGetValue(channelId, out CacheEntry<ImmutableArray<Emote>> entry) && entry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = entry.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public void AddGlobalEmotes(ImmutableArray<Emote> emotes) => _globalEmoteCache = new(emotes);

    public bool TryGetGlobalEmotes(out ImmutableArray<Emote> emotes)
    {
        if (_globalEmoteCache.IsValid(Options.GlobalEmotesCacheTime))
        {
            emotes = _globalEmoteCache.Value;
            return true;
        }

        emotes = [];
        return false;
    }

    public bool Equals(BttvApiCache? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is BttvApiCache other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(BttvApiCache? left, BttvApiCache? right) => Equals(left, right);

    public static bool operator !=(BttvApiCache? left, BttvApiCache? right) => !(left == right);
}
