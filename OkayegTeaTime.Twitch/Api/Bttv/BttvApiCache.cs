using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Memory;
using OkayegTeaTime.Twitch.Api.Bttv.Models;

namespace OkayegTeaTime.Twitch.Api.Bttv;

public sealed class BttvApiCache : IEquatable<BttvApiCache>
{
    public CacheOptions Options { get; }

    private readonly ConcurrentDictionary<long, CacheEntry<Emote[]>> _channelEmoteCache = new();
    private CacheEntry<Emote[]> _globalEmoteCache = CacheEntry<Emote[]>.Empty;

    public BttvApiCache(CacheOptions options)
    {
        Options = options;
    }

    public void AddChannelEmotes(long channelId, Emote[] emotes)
    {
        _channelEmoteCache.AddOrSet(channelId, new(emotes));
    }

    public bool TryGetChannelEmotes(long channelId, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        if (_channelEmoteCache.TryGetValue(channelId, out var entry) && entry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = entry.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public void AddGlobalEmotes(Emote[] emotes)
    {
        _globalEmoteCache = new(emotes);
    }

    public bool TryGetGlobalEmotes([MaybeNullWhen(false)] out Emote[] emotes)
    {
        if (_globalEmoteCache.IsValid(Options.GlobalEmotesCacheTime))
        {
            emotes = _globalEmoteCache.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public bool Equals(BttvApiCache? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is BttvApiCache other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(BttvApiCache? left, BttvApiCache? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BttvApiCache? left, BttvApiCache? right)
    {
        return !(left == right);
    }
}
