using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Properties;
using SpotifyAPI.Web;
using Sterbehilfe.Time;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OkayegTeaTimeCSharp.Spotify
{
    public static class SpotifyRequest
    {
        public static async Task<string> GetCurrentlyPlaying(string username)
        {
            if (new OkayegTeaTimeContext().Spotify.Any(s => s.Username == username))
            {
                Database.Models.Spotify user = DataBase.GetSpotifyUser(username);
                if (user.Time + new Hour().ToMilliseconds() <= TimeHelper.Now() + 5000)
                {
                    await GetNewAccessToken(username);
                    user = DataBase.GetSpotifyUser(username);
                }
                CurrentlyPlaying response = await new SpotifyClient(user.AccessToken).Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                return response.GetItem() != null ? response.GetItem().Message : "nothing playing";
            }
            else
            {
                return "the user is not registered";
            }
        }

        public static string GetLoginURL()
        {
            LoginRequest login = new(new Uri("https://www.example.com/callback"), Resources.SpotifyClientID, LoginRequest.ResponseType.Code)
            {
                Scope = new[] { Scopes.UserReadCurrentlyPlaying }
            };
            return HttpUtility.UrlDecode(login.ToUri().ToString());
        }

        public static async Task GetNewAccessToken(string username)
        {
            AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, DataBase.GetRefreshToken(username)));
            DataBase.UpdateAccessToken(username, response.AccessToken);
        }

        public static async Task GetNewAuthTokens(string username, string code)
        {
            AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, code, new Uri("https://www.example.com/callback")));
            DataBase.AddNewToken(username, response.AccessToken, response.RefreshToken);
        }
    }
}