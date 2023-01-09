using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using HLE.Collections;
using OkayegTeaTime.Files;
using OkayegTeaTime.Utils;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Chat.Emotes;
using TwitchLib.Api.Helix.Models.Chat.Emotes.GetChannelEmotes;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

#pragma warning disable CS0659

namespace OkayegTeaTime.Twitch;

public sealed class TwitchApi
{
    private readonly TwitchAPI _api = new();

    // kinda scuffed with a list of value tuples, but comparision of keys in a dictionary didnt work (for Dictionary<UserKey, User>)
    private readonly List<(UserKey Key, User User)> _users = new();

    public TwitchApi()
    {
        _api.Settings.ClientId = AppSettings.Twitch.ApiClientId;
        _api.Settings.Secret = AppSettings.Twitch.ApiClientSecret;
        _api.Settings.AccessToken = GetAccessToken();
        _api.Settings.Scopes = new()
        {
            AuthScopes.Channel_Check_Subscription,
            AuthScopes.Channel_Subscriptions,
            AuthScopes.Helix_Channel_Read_Subscriptions,
            AuthScopes.User_Subscriptions
        };
    }

    private User? GetUserFromCache(UserKey key)
    {
        (UserKey Key, User User) tuple = _users.FirstOrDefault(u => u.Key.Equals(key));
        return tuple == default ? null : tuple.User;
    }

    private string GetAccessToken()
    {
        HttpPost request = new("https://id.twitch.tv/oauth2/token", new[]
        {
            ("client_id", _api.Settings.ClientId),
            ("client_secret", _api.Settings.Secret),
            ("grant_type", "client_credentials")
        });

        if (request.Result is null)
        {
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Result)}");
        }

        JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
        string? accessToken = json.GetProperty("access_token").GetString();
        if (accessToken is not null)
        {
            return accessToken;
        }

        throw new ArgumentNullException(nameof(accessToken));
    }

    public User? GetUser(string username)
    {
        UserKey key = new(username);
        User? user = GetUserFromCache(key);
        if (user is not null)
        {
            return user;
        }

        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: new()
        {
            username
        }).Result;
        user = response.Users.FirstOrDefault();
        if (user is null)
        {
            return null;
        }

        key.Id = long.Parse(user.Id);
        _users.Add((key, user));
        return user;
    }

    public Dictionary<string, User?> GetUsers(IEnumerable<string> usernames)
    {
        string[] usernameArray = usernames.Select(u => u.ToLower()).ToArray();
        string[] cachedUsernames = usernameArray.Where(u => GetUserFromCache(new(u)) is not null).ToArray();
        Dictionary<string, User?> result = cachedUsernames.Select(u => GetUserFromCache(new(u))!).ToDictionary<User, string, User?>(u => u.Login, u => u);
        List<string> notCachedUsernames = usernameArray.Where(u => GetUserFromCache(new(u)) is null).ToList();
        if (notCachedUsernames.Count == 0)
        {
            return result;
        }

        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: notCachedUsernames).Result;
        Dictionary<string, User?> users = notCachedUsernames.ToDictionary(u => u, u => response.Users.FirstOrDefault(uu => uu.Login == u));
        users.Where(p => p.Value is not null).ForEach(p =>
        {
            UserKey key = new(long.Parse(p.Value!.Id), p.Key);
            _users.Add((key, p.Value!));
        });
        return result.Concat(users).ToDictionary(u => u.Key, u => u.Value);
    }

    public User? GetUser(long id)
    {
        UserKey key = new(id);
        User? user = GetUserFromCache(key);
        if (user is not null)
        {
            return user;
        }

        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(new()
        {
            id.ToString()
        }).Result;
        user = response.Users.FirstOrDefault();
        if (user is null)
        {
            return null;
        }

        key.Name = user.Login;
        _users.Add((key, user));
        return user;
    }

    public Dictionary<long, User?> GetUsers(IEnumerable<long> ids)
    {
        long[] arr = ids.ToArray();
        long[] cachedIds = arr.Where(i => GetUserFromCache(new(i)) is not null).ToArray();
        Dictionary<long, User?> result = cachedIds.Select(i => GetUserFromCache(new(i))!).ToDictionary<User, long, User?>(u => long.Parse(u.Id), u => u);
        long[] notCachedIds = arr.Where(i => GetUserFromCache(new(i)) is null).ToArray();
        if (notCachedIds.Length == 0)
        {
            return result;
        }

        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(notCachedIds.Select(i => i.ToString()).ToList()).Result;
        Dictionary<long, User?> users = notCachedIds.ToDictionary(id => id, id => response.Users.FirstOrDefault(u => long.Parse(u.Id) == id));
        users.Where(p => p.Value is not null).ForEach(p =>
        {
            UserKey key = new(p.Key, p.Value!.Login);
            _users.Add((key, p.Value!));
        });
        return result.Concat(users).ToDictionary(u => u.Key, u => u.Value);
    }

    public long GetUserId(string username)
    {
        User? user = GetUser(username);
        return user?.Id is not null ? long.Parse(user.Id) : -1;
    }

    public bool DoesUserExist(string username)
    {
        return GetUser(username) is not null;
    }

    public Dictionary<string, bool> DoUsersExist(IEnumerable<string> usernames)
    {
        Dictionary<string, User?> users = GetUsers(usernames);
        IEnumerable<KeyValuePair<string, bool>> result = users.Select(u => new KeyValuePair<string, bool>(u.Key, u.Value is not null));
        return new(result);
    }

    public bool DoesUserExist(long id)
    {
        return GetUser(id) is not null;
    }

    public Dictionary<long, bool> DoUsersExist(IEnumerable<long> ids)
    {
        Dictionary<long, User?> users = GetUsers(ids);
        IEnumerable<KeyValuePair<long, bool>> result = users.Select(u => new KeyValuePair<long, bool>(u.Key, u.Value is not null));
        return new(result);
    }

    public Stream? GetStream(string channel)
    {
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userLogins: new()
        {
            channel
        }).Result;
        return response.Streams.FirstOrDefault();
    }

    public Stream? GetStream(long id)
    {
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userIds: new()
        {
            id.ToString()
        }).Result;
        return response.Streams.FirstOrDefault();
    }

    public bool IsLive(string channel)
    {
        return GetStream(channel) is not null;
    }

    public bool IsLive(long id)
    {
        return GetStream(id) is not null;
    }

    public ChannelEmote[] GetSubEmotes(string channel)
    {
        long channelId = GetUserId(channel);
        return channelId == -1 ? Array.Empty<ChannelEmote>() : GetSubEmotes(channelId);
    }

    public ChannelEmote[] GetSubEmotes(long channelId)
    {
        GetChannelEmotesResponse response = _api.Helix.Chat.GetChannelEmotesAsync(channelId.ToString()).Result;
        return response.ChannelEmotes;
    }

    [DebuggerDisplay("Id = {Id} Name = \"{Name}\"")]
    private sealed class UserKey
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public UserKey(long id)
        {
            Id = id;
        }

        public UserKey(string name)
        {
            Name = name;
        }

        public UserKey(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is UserKey key && (key.Id != default && Id != default && key.Id == Id || key.Name != default && Name != default && key.Name == Name);
        }
    }
}
