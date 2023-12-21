using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Collections.Concurrent;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Helix;

public sealed class TwitchApiCache(CacheOptions options)
{
    public CacheOptions Options { get; set; } = options;

    private readonly ConcurrentDoubleDictionary<long, int, User> _userCache = new();
    private readonly ConcurrentDoubleDictionary<long, int, Stream> _streamCache = new();
    private CacheEntry<ImmutableArray<Emote>> _globalEmoteCache = CacheEntry<ImmutableArray<Emote>>.Empty;
    private readonly ConcurrentDictionary<long, CacheEntry<ImmutableArray<ChannelEmote>>> _channelEmoteCache = new();

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
        => _userCache.TryGetByPrimaryKey(userId, out user) && user.IsValid(Options.UserCacheTime);

    public bool TryGetUser(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out User user)
    {
        int usernameHash = string.GetHashCode(username, StringComparison.OrdinalIgnoreCase);
        return _userCache.TryGetBySecondaryKey(usernameHash, out user) && user.IsValid(Options.UserCacheTime);
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
        => _streamCache.TryGetByPrimaryKey(channelId, out stream) && stream.IsValid(Options.StreamCacheTime);

    public bool TryGetStream(ReadOnlySpan<char> username, [MaybeNullWhen(false)] out Stream stream)
    {
        int usernameHash = string.GetHashCode(username, StringComparison.OrdinalIgnoreCase);
        return _streamCache.TryGetBySecondaryKey(usernameHash, out stream) && stream.IsValid(Options.StreamCacheTime);
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

    public void AddChannelEmotes(long channelId, ImmutableArray<ChannelEmote> emotes) => _channelEmoteCache.AddOrSet(channelId, new(emotes));

    public bool TryGetChannelEmotes(long channelId, out ImmutableArray<ChannelEmote> emotes)
    {
        if (_channelEmoteCache.TryGetValue(channelId, out CacheEntry<ImmutableArray<ChannelEmote>> entry) && entry.IsValid(Options.ChannelEmotesCacheTime))
        {
            emotes = entry.Value;
            return true;
        }

        emotes = [];
        return false;
    }
}
