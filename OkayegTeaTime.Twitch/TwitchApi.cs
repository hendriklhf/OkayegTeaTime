using System;
using System.Collections.Generic;
using System.Linq;
using HLE;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Chat.Emotes;
using TwitchLib.Api.Helix.Models.Chat.Emotes.GetChannelEmotes;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;
using TwitchLibApi = TwitchLib.Api.TwitchAPI;

namespace OkayegTeaTime.Twitch;

public class TwitchApi
{
    private readonly TwitchLibApi _api = new();

    public TwitchApi()
    {
        _api.Settings.ClientId = AppSettings.Twitch.ApiClientId;
        _api.Settings.Secret = AppSettings.Twitch.ApiClientSecret;
        _api.Settings.Scopes = new()
        {
            AuthScopes.Channel_Check_Subscription,
            AuthScopes.Channel_Subscriptions,
            AuthScopes.Helix_Channel_Read_Subscriptions,
            AuthScopes.User_Subscriptions
        };
        RefreshAccessToken();
    }

    private string GetAccessToken()
    {
        HttpPost request = new("https://id.twitch.tv/oauth2/token", new[]
        {
            ("client_id", _api.Settings.ClientId),
            ("client_secret", _api.Settings.Secret),
            ("grant_type", "client_credentials")
        });

        string? accessToken = request.IsValidJsonData ? request.Data.GetProperty("access_token").GetString() : null;
        if (accessToken is not null)
        {
            return accessToken;
        }

        ArgumentNullException ex = new(nameof(accessToken));
        DbController.LogException(ex);
        throw ex;
    }

    public void RefreshAccessToken()
    {
        _api.Settings.AccessToken = GetAccessToken();
    }

    public User? GetUser(string username)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: new()
        {
            username
        }).Result;
        return response.Users.FirstOrDefault();
    }

    public Dictionary<string, User?> GetUsers(IEnumerable<string> usernames)
    {
        List<string> users = usernames.ToList();
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: users).Result;
        return users.ToDictionary(username => username, username => response.Users.FirstOrDefault(u => string.Equals(u.DisplayName, username, StringComparison.CurrentCultureIgnoreCase)));
    }

    public User? GetUser(long id)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(ids: new()
        {
            id.ToString()
        }).Result;
        return response.Users.FirstOrDefault();
    }

    public Dictionary<long, User?> GetUsers(IEnumerable<long> ids)
    {
        List<long> idss = ids.ToList();
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(ids: idss.Select(i => i.ToString()).ToList()).Result;
        return idss.ToDictionary(id => id, id => response.Users.FirstOrDefault(u => u.Id.ToLong() == id));
    }

    public long? GetUserId(string username)
    {
        return GetUser(username)?.Id?.ToLong();
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
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userLogins: new List<string>
        {
            channel
        }).Result;
        return response.Streams.FirstOrDefault();
    }

    public Stream? GetStream(long id)
    {
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userIds: new List<string>
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
        long? channelId = GetUserId(channel);
        return channelId is null ? Array.Empty<ChannelEmote>() : GetSubEmotes(channelId.Value);
    }

    public ChannelEmote[] GetSubEmotes(long channelId)
    {
        GetChannelEmotesResponse response = _api.Helix.Chat.GetChannelEmotesAsync(channelId.ToString()).Result;
        return response.ChannelEmotes;
    }
}
