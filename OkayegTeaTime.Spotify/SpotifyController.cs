using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using OkayegTeaTime.Files;
using OkayegTeaTime.Utils;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyController
{
    public static string GetLoginUrl()
    {
        LoginRequest login = new(new("https://example.com/callback"), AppSettings.Spotify.ClientId, LoginRequest.ResponseType.Code)
        {
            Scope = new[]
            {
                Scopes.UserReadPlaybackState,
                Scopes.UserModifyPlaybackState
            }
        };

        return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20");
    }

    public static async Task<string?> GetNewAccessToken(string refreshToken)
    {
        try
        {
            AuthorizationCodeRefreshResponse response =
                await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, refreshToken));
            return response.AccessToken;
        }
        catch (Exception)
        {
            // log exception if needed
            return null;
        }
    }

    public static async Task<(string AccessToken, string RefreshToken)?> GetNewAuthTokens(string code)
    {
        try
        {
            AuthorizationCodeTokenResponse response =
                await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, code, new("https://example.com/callback")));
            return (response.AccessToken, response.RefreshToken);
        }
        catch (Exception)
        {
            // log exception if needed
            return null;
        }
    }

    public static string? ParseSongToUri(string input)
    {
        if (Pattern.SpotifyUri.IsMatch(input))
        {
            return input;
        }

        if (Pattern.SpotifyLink.IsMatch(input))
        {
            string uriCode = Regex.Match(input, @"track/\w+").Value[6..];
            return $"spotify:track:{uriCode}";
        }

        return Regex.IsMatch(input, @"^\w{22}$") ? $"spotify:track:{input}" : null;
    }
}
