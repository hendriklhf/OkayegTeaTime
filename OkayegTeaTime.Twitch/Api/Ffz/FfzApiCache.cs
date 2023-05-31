using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections.Concurrent;
using HLE.Memory;
using OkayegTeaTime.Twitch.Api.Ffz.Models;

namespace OkayegTeaTime.Twitch.Api.Ffz;

public sealed class FfzApiCache : IEquatable<FfzApiCache>
{
    public CacheOptions Options { get; set; }

    private readonly ConcurrentDoubleDictionary<long, int, CacheEntry<Emote[]>> _channelEmotesCache = new();
    private CacheEntry<Emote[]> _globalEmotesCache = CacheEntry<Emote[]>.Empty;

    public FfzApiCache(CacheOptions options)
    {
        Options = options;
    }

    public void AddChannelEmotes(long channelId, ReadOnlySpan<char> channelName, Emote[] emotes)
    {
        int channelNameHash = string.GetHashCode(channelName, StringComparison.OrdinalIgnoreCase);
        _channelEmotesCache.AddOrSet(channelId, channelNameHash, new(emotes));
    }

    public void AddGlobalEmotes(Emote[] emotes)
    {
        _globalEmotesCache = new(emotes);
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
        if (_channelEmotesCache.TryGetValue(channelId, out CacheEntry<Emote[]> emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public bool TryGetChannelEmotes(ReadOnlySpan<char> channelName, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        int channelNameHash = string.GetHashCode(channelName, StringComparison.OrdinalIgnoreCase);
        if (_channelEmotesCache.TryGetValue(channelNameHash, out CacheEntry<Emote[]> emoteEntry) && emoteEntry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = emoteEntry.Value;
            return true;
        }

        emotes = null;
        return false;
    }

    public bool Equals(FfzApiCache? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is FfzApiCache other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(FfzApiCache? left, FfzApiCache? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FfzApiCache? left, FfzApiCache? right)
    {
        return !(left == right);
    }
}
