using OkayegTeaTimeCSharp.Properties;
using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;
using System.Web;

namespace OkayegTeaTimeCSharp.Spotify
{
    public static class SpotifyRequest
    {
        public static string GetLoginURL()
        {
            LoginRequest login = new(new Uri("https://www.example.com/callback"), Resources.SpotifyClientID, LoginRequest.ResponseType.Code)
            {
                Scope = new[] { Scopes.UserReadCurrentlyPlaying }
            };
            return HttpUtility.UrlDecode(login.ToUri().ToString());
        }

        public static async Task GetNewAuthTokens(string username, string code)
        {
            AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, code, new Uri("https://www.example.com/callback")));
        }

        public static async Task GetNewAccesToken(string username, string refreshToken)
        {
            AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, refreshToken));
        }

        public static async Task GetCurrentlyPlaying(string username, string accessToken)
        {
            CurrentlyPlaying response = await new SpotifyClient(accessToken).Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
        }
    }
}