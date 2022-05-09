using System.Threading.Tasks;
using System.Web;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Logging;
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

    public static async Task<string?> GetNewAccessToken(string username)
    {
        Database.Models.SpotifyUser? user = DbControl.SpotifyUsers[username];
        if (user is null)
        {
            return null;
        }

        try
        {
            AuthorizationCodeRefreshResponse response =
                await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, user.RefreshToken));
            return response.AccessToken;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return null;
        }
    }

    public static async Task GetNewAuthTokens(string username, string code)
    {
        try
        {
            AuthorizationCodeTokenResponse response =
                await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, code, new("https://example.com/callback")));
            DbController.AddSpotifyUser(username, response.AccessToken, response.RefreshToken);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
        }
    }

    public static string? ParseSongToUri(string input)
    {
        if (input.IsMatch(Pattern.SpotifyUri))
        {
            return input;
        }

        if (input.IsMatch(Pattern.SpotifyLink))
        {
            string uriCode = input.Match(@"track/\w+")[6..];
            return $"spotify:track:{uriCode}";
        }

        if (input.IsMatch(@"^\w{22}$"))
        {
            return $"spotify:track:{input}";
        }

        return null;
    }
}
