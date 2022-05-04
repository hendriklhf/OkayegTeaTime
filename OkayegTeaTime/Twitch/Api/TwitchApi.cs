using HLE.HttpRequests;
using HLE.Strings;
using OkayegTeaTime.Logging;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Chat.Emotes;
using TwitchLib.Api.Helix.Models.Chat.Emotes.GetChannelEmotes;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLibApi = TwitchLib.Api.TwitchAPI;

namespace OkayegTeaTime.Twitch.Api;

public static class TwitchApi
{
    private static readonly TwitchLibApi _api = new();

    public static void Initialize()
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
        string? accessToken = GetAccessToken();
        if (accessToken is null)
        {
            ArgumentNullException ex = new(nameof(accessToken));
            Logger.Log(ex);
            throw ex;
        }

        _api.Settings.AccessToken = accessToken;
    }

    private static string? GetAccessToken()
    {
        HttpPost request = new("https://id.twitch.tv/oauth2/token", new[]
        {
            ("client_id", _api.Settings.ClientId),
            ("client_secret", _api.Settings.Secret),
            ("grant_type", "client_credentials")
        });

        if (!request.IsValidJsonData)
        {
            return null;
        }

        return request.Data.GetProperty("access_token").GetString();
    }

    public static void RefreshAccessToken()
    {
        _api.Settings.AccessToken = GetAccessToken();
    }

    public static User? GetUser(string username)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: new()
        {
            username
        }).Result;
        return response.Users.FirstOrDefault();
    }

    public static Dictionary<string, User?> GetUsers(IEnumerable<string> usernames)
    {
        List<string> users = usernames.ToList();
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: users).Result;
        return users.ToDictionary(username => username, username => response.Users.FirstOrDefault(u => string.Equals(u.DisplayName, username, StringComparison.CurrentCultureIgnoreCase)));
    }

    public static User? GetUser(int id)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(ids: new()
        {
            id.ToString()
        }).Result;
        return response.Users.FirstOrDefault();
    }

    public static Dictionary<int, User?> GetUsers(IEnumerable<int> ids)
    {
        List<int> idss = ids.ToList();
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(ids: idss.Select(i => i.ToString()).ToList()).Result;
        return idss.ToDictionary(id => id, id => response.Users.FirstOrDefault(u => u.Id.ToInt() == id));
    }

    public static int? GetUserId(string username)
    {
        return GetUser(username)?.Id?.ToInt();
    }

    public static bool DoesUserExist(string username)
    {
        return GetUser(username) is not null;
    }

    public static Dictionary<string, bool> DoUsersExist(IEnumerable<string> usernames)
    {
        Dictionary<string, User?> users = GetUsers(usernames);
        IEnumerable<KeyValuePair<string, bool>> result = users.Select(u => new KeyValuePair<string, bool>(u.Key, u.Value is not null));
        return new(result);
    }

    public static bool DoesUserExist(int id)
    {
        return GetUser(id) is not null;
    }

    public static Dictionary<int, bool> DoUsersExist(IEnumerable<int> ids)
    {
        Dictionary<int, User?> users = GetUsers(ids);
        IEnumerable<KeyValuePair<int, bool>> result = users.Select(u => new KeyValuePair<int, bool>(u.Key, u.Value is not null));
        return new(result);
    }

    public static Stream? GetStream(string channel)
    {
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userLogins: new List<string>
        {
            channel
        }).Result;
        return response.Streams.FirstOrDefault();
    }

    public static Stream? GetStream(int id)
    {
        GetStreamsResponse response = _api.Helix.Streams.GetStreamsAsync(userIds: new List<string>
        {
            id.ToString()
        }).Result;
        return response.Streams.FirstOrDefault();
    }

    public static bool IsLive(string channel)
    {
        return GetStream(channel) is not null;
    }

    public static bool IsLive(int id)
    {
        return GetStream(id) is not null;
    }

    public static ChannelEmote[] GetSubEmotes(string channel)
    {
        int? channelId = GetUserId(channel);
        return channelId is null ? Array.Empty<ChannelEmote>() : GetSubEmotes(channelId.Value);
    }

    public static ChannelEmote[] GetSubEmotes(int channelId)
    {
        GetChannelEmotesResponse response = _api.Helix.Chat.GetChannelEmotesAsync(channelId.ToString()).Result;
        return response.ChannelEmotes;
    }
}
