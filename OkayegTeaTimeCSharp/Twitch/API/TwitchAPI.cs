using HLE.HttpRequests;
using OkayegTeaTimeCSharp.Properties;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLibApi = TwitchLib.Api.TwitchAPI;

namespace OkayegTeaTimeCSharp.Twitch.Api;

public static class TwitchApi
{
    private static readonly TwitchLibApi _api = new();

    public static void Configure()
    {
        _api.Settings.ClientId = Settings.TwitchApiClientID;
        _api.Settings.Secret = Settings.TwitchApiClientSecret;
        _api.Settings.Scopes = new()
        {
            AuthScopes.Channel_Check_Subscription,
            AuthScopes.Channel_Subscriptions,
            AuthScopes.Helix_Channel_Read_Subscriptions,
            AuthScopes.User_Subscriptions
        };
        _api.Settings.AccessToken = GetAccessToken();
    }

    private static string GetAccessToken()
    {
        HttpPost request = new("https://id.twitch.tv/oauth2/token",
            new()
            {
                new("client_id", _api.Settings.ClientId),
                new("client_secret", _api.Settings.Secret),
                new("grant_type", "client_credentials")
            });
        return request.Data.GetProperty("access_token").GetString();
    }

    public static void RefreshAccessToken()
    {
        _api.Settings.AccessToken = GetAccessToken();
    }

    public static User GetUserByName(string username)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(logins: new() { username }).Result;
        if (response?.Users?.Length > 0)
        {
            return response.Users[0];
        }
        else
        {
            return null;
        }
    }

    public static User GetUserById(int id)
    {
        GetUsersResponse response = _api.Helix.Users.GetUsersAsync(ids: new() { $"{id}" }).Result;
        if (response?.Users?.Length > 0)
        {
            return response.Users[0];
        }
        else
        {
            return null;
        }
    }

    public static string GetUserId(string username)
    {
        return GetUserByName(username)?.Id;
    }

    public static bool DoesUserExsist(string username)
    {
        return GetUserByName(username) is not null;
    }
}
