using System.Threading.Tasks;
using System.Web;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using SpotifyAPI.Web;
using SpotifyModel = OkayegTeaTime.Database.Models.Spotify;

namespace OkayegTeaTime.Spotify;

public static class SpotifyController
{
    public static string GetLoginUrl()
    {
        LoginRequest login = new(new("https://example.com/callback"), AppSettings.Spotify.ClientId, LoginRequest.ResponseType.Code)
        {
            Scope = new[] { Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState }
        };
        return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20");
    }

    public static async Task GetNewAccessToken(string username)
    {
        string? refreshToken = DbController.GetRefreshToken(username);
        if (refreshToken is null)
        {
            return;
        }

        AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, refreshToken));
        DbController.UpdateAccessToken(username, response.AccessToken);
    }

    public static async Task GetNewAuthTokens(string username, string code)
    {
        AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, code, new("https://example.com/callback")));
        DbController.AddNewToken(username, response.AccessToken, response.RefreshToken);
    }

    public static async Task<SpotifyUser?> GetSpotifyUser(string username)
    {
        SpotifyModel? user = DbController.GetSpotifyUser(username);
        if (user is null)
        {
            return null;
        }

        if (user.Time + new Hour().Milliseconds <= TimeHelper.Now() + new Second(5).Milliseconds)
        {
            await GetNewAccessToken(username);
            user = DbController.GetSpotifyUser(username);
        }

        return user is null ? null : new(user);
    }

    public static FullTrack? GetExcactTrackFromSearch(List<FullTrack>? tracks, List<string> query)
    {
        return tracks?.FirstOrDefault(t =>
            query.Any(q => t.Name.IsMatch(q))
            || query.Any(q => t.Artists.Any(a => a.Name.IsMatch(q))));
    }

    public static string? ParseSongToUri(string input)
    {
        if (input.IsMatch(Pattern.SpotifyUri))
        {
            return input;
        }
        else if (input.IsMatch(Pattern.SpotifyLink))
        {
            string uriCode = input.Match(@"track/\w+\?").Remove("track/").Remove("?");
            return $"spotify:track:{uriCode}";
        }
        else if (input.IsMatch(@"^\w{22}$"))
        {
            return $"spotify:track:{input}";
        }
        else
        {
            return null;
        }
    }
}
