using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Database.Models;
using OkayegTeaTimeCSharp.Properties;
using SpotifyAPI.Web;
using Sterbehilfe.Strings;
using Sterbehilfe.Time;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OkayegTeaTimeCSharp.Spotify
{
    public static class SpotifyRequest
    {
        public static async Task<string> AddToQueue(string channel, string song)
        {
            song = SpotifyHelper.GetSpotifyURI(song);
            if (song == null)
            {
                return "this isn't a valid track link";
            }
            if (new OkayegTeaTimeContext().Spotify.Any(s => s.Username == channel))
            {
                Database.Models.Spotify user = await GetSpotifyUser(channel);
                if (user.SongRequestEnabled == true)
                {
                    SpotifyClient client = new(user.AccessToken);
                    await client.Player.AddToQueue(new PlayerAddToQueueRequest(song));
                    FullTrack item = await client.Tracks.Get(song.Remove("spotify:track:"));
                    return $"{item.Name} by {item.Artists.GetArtistString()} has been added to the queue";
                }
                else
                {
                    return $"song requests are currently not open, {channel} has to enable song requests first";
                }
            }
            else
            {
                return $"can't add the song to the queue of {channel}, they have to register first";
            }
        }

        public static async Task<string> GetCurrentlyPlaying(string username)
        {
            if (new OkayegTeaTimeContext().Spotify.Any(s => s.Username == username))
            {
                Database.Models.Spotify user = await GetSpotifyUser(username);
                CurrentlyPlaying response = await new SpotifyClient(user.AccessToken).Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                return response.GetItem() != null ? response.GetItem().Message : "nothing playing";
            }
            else
            {
                return $"can't request the current playing song, user {username} has to register first";
            }
        }
        public static string GetLoginURL()
        {
            LoginRequest login = new(new Uri("https://example.com/callback"), Resources.SpotifyClientID, LoginRequest.ResponseType.Code)
            {
                Scope = new[] { Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState }
            };
            return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20");
        }

        public static async Task GetNewAccessToken(string username)
        {
            AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, DataBase.GetRefreshToken(username)));
            DataBase.UpdateAccessToken(username, response.AccessToken);
        }

        public static async Task GetNewAuthTokens(string username, string code)
        {
            AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, code, new Uri("https://example.com/callback")));
            DataBase.AddNewToken(username, response.AccessToken, response.RefreshToken);
        }

        public static async Task<Database.Models.Spotify> GetSpotifyUser(string username)
        {
            Database.Models.Spotify user = DataBase.GetSpotifyUser(username);
            if (user.Time + new Hour().Milliseconds <= TimeHelper.Now() + new Second(5).Milliseconds)
            {
                await GetNewAccessToken(username);
                return DataBase.GetSpotifyUser(username);
            }
            else
            {
                return user;
            }
        }
    }
}