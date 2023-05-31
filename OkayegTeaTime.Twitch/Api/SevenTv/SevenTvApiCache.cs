using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Memory;
using OkayegTeaTime.Twitch.Api.SevenTv.Models;

namespace OkayegTeaTime.Twitch.Api.SevenTv;

public sealed class SevenTvApiCache : IEquatable<SevenTvApiCache>
{
    public CacheOptions Options { get; set; }

    private CacheEntry<Emote[]> _globalEmotesCache = CacheEntry<Emote[]>.Empty;
    private readonly ConcurrentDictionary<long, CacheEntry<Emote[]>> _channelEmotesCache = new();

    public SevenTvApiCache(CacheOptions options)
    {
        Options = options;
    }

    public void AddGlobalEmotes(Emote[] emotes)
    {
        _globalEmotesCache = new(emotes);
    }

    public void AddChannelEmotes(long channelId, Emote[] emotes)
    {
        _channelEmotesCache.AddOrSet(channelId, new(emotes));
    }

    public bool TryGetGlobalEmotes([MaybeNullWhen(false)] out Emote[] emotes)
    {
        if (_globalEmotesCache.IsValid(Options.GlobalEmotesCacheTime))
        {
            emotes = _globalEmotesCache.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public bool TryGetChannelEmotes(long channelId, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        if (_channelEmotesCache.TryGetValue(channelId, out var emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public bool Equals(SevenTvApiCache? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is SevenTvApiCache other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(SevenTvApiCache? left, SevenTvApiCache? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SevenTvApiCache? left, SevenTvApiCache? right)
    {
        return !(left == right);
    }
}
