using System.Threading.Tasks;
using System.Web;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Logging;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyRequest
{
    public static async Task<string> AddToQueue(string channel, string song)
    {
        song = SpotifyHelper.GetSpotifyURI(song);
        if (song is null)
        {
            return "this isn't a valid track link";
        }
        if (DatabaseController.DoesSpotifyUserExist(channel))
        {
            Database.Models.Spotify user = await GetSpotifyUser(channel);
            if (user.SongRequestEnabled == true)
            {
                try
                {
                    SpotifyClient client = new(user.AccessToken);
                    await client.Player.AddToQueue(new(song));
                    FullTrack item = await client.Tracks.Get(song.Remove("spotify:track:"));
                    return $"{item.Name} by {item.Artists.GetArtists()} has been added to the queue";
                }
                catch (APIException ex)
                {
                    Logger.Log(ex);
                    return $"no music playing on any device, {channel} has to start their playback first";
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return $"that song id doesn't match any song";
                }
            }
            else
            {
                return $"song requests are currently not open, {channel} or a mod has to enable song requests first";
            }
        }
        else
        {
            return $"can't add the song to the queue of {channel}, they have to register first";
        }
    }

    public static async Task<string> GetCurrentlyPlaying(string username)
    {
        if (DatabaseController.DoesSpotifyUserExist(username))
        {
            Database.Models.Spotify user = await GetSpotifyUser(username);
            CurrentlyPlaying response = await new SpotifyClient(user.AccessToken).Player.GetCurrentlyPlaying(new());
            return response.GetItem() is not null ? response.GetItem().Message : "nothing playing";
        }
        else
        {
            return $"can't request the current playing song, user {username} has to register first";
        }
    }

    public static string GetLoginURL()
    {
        LoginRequest login = new(new("https://example.com/callback"), AppSettings.Spotify.ClientId, LoginRequest.ResponseType.Code)
        {
            Scope = new[] { Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState }
        };
        return HttpUtility.UrlDecode(login.ToUri().AbsoluteUri).Replace(" ", "%20");
    }

    public static async Task GetNewAccessToken(string username)
    {
        AuthorizationCodeRefreshResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, DatabaseController.GetRefreshToken(username)));
        DatabaseController.UpdateAccessToken(username, response.AccessToken);
    }

    public static async Task GetNewAuthTokens(string username, string code)
    {
        AuthorizationCodeTokenResponse response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(AppSettings.Spotify.ClientId, AppSettings.Spotify.ClientSecret, code, new("https://example.com/callback")));
        DatabaseController.AddNewToken(username, response.AccessToken, response.RefreshToken);
    }

    public static async Task<Database.Models.Spotify> GetSpotifyUser(string username)
    {
        Database.Models.Spotify user = DatabaseController.GetSpotifyUser(username);
        if (user.Time + new Hour().Milliseconds <= TimeHelper.Now() + new Second(5).Milliseconds)
        {
            await GetNewAccessToken(username);
            return DatabaseController.GetSpotifyUser(username);
        }
        else
        {
            return user;
        }
    }

    public static async Task<string> Search(string query)
    {
        Database.Models.Spotify user = await GetSpotifyUser(AppSettings.UserLists.Owners.First());
        SearchResponse response = await new SpotifyClient(user.AccessToken).Search.Item(new(SearchRequest.Types.Track, query));
        if (response.Tracks.Items.Count > 0)
        {
            FullTrack track = SpotifyHelper.GetExcactTrackFromSearch(response.Tracks.Items, query.Split().ToList());
            return $"{track.Name} by {track.Artists.GetArtists()} || {track.Uri}";
        }
        else
        {
            return "no tracks found to match the search query";
        }
    }

    public static async Task<string> SkipToNextSong(string channel)
    {
        if (DatabaseController.DoesSpotifyUserExist(channel))
        {
            Database.Models.Spotify user = await GetSpotifyUser(channel);
            if (user.SongRequestEnabled == true)
            {
                try
                {
                    await new SpotifyClient(user.AccessToken).Player.SkipNext(new());
                    return $"skipped to the next song in queue";
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    return $"couldn't skip to the next song";
                }
            }
            else
            {
                return $"song requests are currently not open, {channel} or a mod has to enable song requests first";
            }
        }
        else
        {
            return $"can't skip a song of {channel}, they have to register first";
        }
    }
}
