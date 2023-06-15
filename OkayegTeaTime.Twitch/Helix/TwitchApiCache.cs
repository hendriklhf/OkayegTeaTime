using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Collections.Concurrent;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Helix;

public sealed class TwitchApiCache
{
    public CacheOptions Options { get; set; }

    private readonly ConcurrentDoubleDictionary<long, int, User> _userCache = new();
    private readonly ConcurrentDoubleDictionary<long, int, Stream> _streamCache = new();
    private CacheEntry<Emote[]> _globalEmoteCache = CacheEntry<Emote[]>.Empty;
    private readonly ConcurrentDictionary<long, CacheEntry<ChannelEmote[]>> _channelEmoteCache = new();

    public TwitchApiCache(CacheOptions options)
    {
        Options = options;
    }

    public void AddUser(User user)
    {
        int usernameHash = string.GetHashCode(user.Username, StringComparison.OrdinalIgnoreCase);
        _userCache.AddOrSet(user.Id, usernameHash, user);
    }

    public void AddUsers(ReadOnlySpan<User> users)
    {
        for (int i = 0; i < users.Length; i++)
        {
            AddUser(users[i]);
        }
    }

    public bool TryGetUser(long userId, [MaybeNullWhen(false)] out User user)
    {
        return _userCache.TryGetValue(userId, out user) && user.IsValid(Options.UserCacheTime);
    }

    public bool TryGetUser(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out User user)
    {
        int usernameHash = string.GetHashCode(username, StringComparison.OrdinalIgnoreCase);
        return _userCache.TryGetValue(usernameHash, out user) && user.IsValid(Options.UserCacheTime);
    }

    public void AddStream(Stream stream)
    {
        int usernameHash = string.GetHashCode(stream.Username, StringComparison.OrdinalIgnoreCase);
        _streamCache.AddOrSet(stream.UserId, usernameHash, stream);
    }

    public void AddStreams(ReadOnlySpan<Stream> streams)
    {
        for (int i = 0; i < streams.Length; i++)
        {
            AddStream(streams[i]);
        }
    }

    public bool TryGetStream(long channelId, [MaybeNullWhen(false)] out Stream stream)
    {
        return _streamCache.TryGetValue(channelId, out stream) && stream.IsValid(Options.StreamCacheTime);
    }

    public bool TryGetStream(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out Stream stream)
    {
        int usernameHash = string.GetHashCode(username, StringComparison.OrdinalIgnoreCase);
        return _streamCache.TryGetValue(usernameHash, out stream) && stream.IsValid(Options.StreamCacheTime);
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

    public void AddChannelEmotes(long channelId, ChannelEmote[] emotes)
    {
        _channelEmoteCache.AddOrSet(channelId, new(emotes));
    }

    public bool TryGetChannelEmotes(long channelId, [MaybeNullWhen(false)] out ChannelEmote[] emotes)
    {
        if (_channelEmoteCache.TryGetValue(channelId, out CacheEntry<ChannelEmote[]> entry) && entry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = entry.Value;
            return true;
        }

        emotes = null;
        return false;
    }
}
