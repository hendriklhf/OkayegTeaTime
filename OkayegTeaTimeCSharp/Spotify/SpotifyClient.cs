using OkayegTeaTimeCSharp.Properties;
using SpotifyAPI.Web;
using System;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class SpotifyRequest
    {
        public static string GetLoginURL()
        {
            LoginRequest login = new(new Uri("https://example.com/callback"), Resources.SpotifyClientID, LoginRequest.ResponseType.Code)
            {
#warning add neeeded scopes
                Scope = new[] { Scopes.UserReadCurrentlyPlaying, "" }
            };
            return login.ToUri().ToString();
        }

        public static async Task GetNewAuthToken(string username, string code)
        {
            AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, code, new Uri("example.com/callback")));
        }

        public static async Task GetNewAccesToken(string username, string refreshToken)
        {
            AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, refreshToken));
        }

        public static string GetCurrentlyPlaying(string username, string accesToken)
        {
            return new SpotifyClient(accesToken).Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest()).Result.Context.Type;
        }
    }
}
