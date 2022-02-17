using System.Threading.Tasks;
using System.Web;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Logging;
using SpotifyAPI.Web;
using SpotifyUser = OkayegTeaTime.Database.Models.Spotify;

namespace OkayegTeaTime.Spotify;

public static class SpotifyRequest
{
    public static async Task<string> AddToQueue(string channel, string song, bool channelEqualsTarget = true)
    {
        string? uri = SpotifyHelper.GetSpotifyUri(song);
        if (uri is null)
        {
            return "this isn't a valid track link";
        }

        SpotifyUser? user = await GetSpotifyUser(channel);
        if (user is null)
        {
            return $"can't add the song to the queue of {channel}, they have to register first";
        }

        if (user.SongRequestEnabled == true)
        {
            try
            {
                SpotifyClient client = new(user.AccessToken);
                await client.Player.AddToQueue(new(uri));
                FullTrack item = await client.Tracks.Get(uri.Remove("spotify:track:"));
                string[] artists = item.Artists.GetArtistNames();
                string response = $"{item.Name} by {string.Join(", ", artists)} has been added to the queue";

                if (!channelEqualsTarget)
                {
                    response = string.Concat(response, $" of {channel}");
                }
                return response;
            }
            catch (APIException ex)
            {
                Logger.Log(ex);
                return $"no music playing on any device, {channel} has to start their playback first";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return $"an unknown error occurred. It might not be possible to request songs for this user";
            }
        }
        else
        {
            return $"song requests are currently not open, {channel} or a mod has to enable song requests first";
        }
    }

    public static async Task<string> GetCurrentlyPlaying(string username)
    {
        SpotifyUser? user = await GetSpotifyUser(username);
        if (user is null)
        {
            return $"can't request the current playing song, user {username} has to register first";
        }

        CurrentlyPlaying response = await new SpotifyClient(user.AccessToken).Player.GetCurrentlyPlaying(new());
        PlayingItem? item = SpotifyHelper.GetPlayingItem(response);
        if (item is not null)
        {
            return item.Message;
        }
        else
        {
            return "nothing playing";
        }
    }

    public static async Task<PlayingItem?> GetCurrentlyPlayingTrack(string username)
    {
        SpotifyUser? user = await GetSpotifyUser(username);
        if (user is null)
        {
            return null;
        }
        
        CurrentlyPlaying response = await new SpotifyClient(user.AccessToken).Player.GetCurrentlyPlaying(new());
        PlayingItem? item = SpotifyHelper.GetPlayingItem(response);
        if (item is not null)
        {
            return item;
        }
    }

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
        SpotifyUser? user = DbController.GetSpotifyUser(username);
        if (user is null)
        {
            return null;
        }

        if (user.Time + new Hour().Milliseconds <= TimeHelper.Now() + new Second(5).Milliseconds)
        {
            await GetNewAccessToken(username);
            return DbController.GetSpotifyUser(username);
        }
        else
        {
            return user;
        }
    }

    public static async Task<string> Search(string query)
    {
        SpotifyUser? user = await GetSpotifyUser("strbhlfe");
        if (user is null)
        {
            return "can't search for a track at the moment";
        }

        SearchResponse response = await new SpotifyClient(user.AccessToken).Search.Item(new(SearchRequest.Types.Track, query));
        FullTrack? track = SpotifyHelper.GetExcactTrackFromSearch(response.Tracks.Items, query.Split().ToList());
        if (track is not null)
        {
            string[] artists = track.Artists.GetArtistNames();
            return $"{track.Name} by {string.Join(", ", artists)} || {track.Uri}";
        }
        else
        {
            return "no tracks found to match the search query";
        }
    }

    public static async Task<string> SkipToNextSong(string channel)
    {
        SpotifyUser? user = await GetSpotifyUser(channel);
        if (user is null)
        {
            return $"can't skip a song of {channel}, they have to register first";
        }

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

    public static async Task<string> ListenTo(string username, string target)
    {
        SpotifyUser? user = await GetSpotifyUser(username);
        if (user is null)
        {
            return $"can't listen to other's songs, you have to register first";
        }

        if (username == target)
        {
            return $"you can't listen to your own song";
        }

        SpotifyUser? targetUser = await GetSpotifyUser(target);
        if (targetUser is null)
        {
            return $"can't listen to {target}'s songs, they have to register first";
        }

        SpotifyClient targetClient = new(targetUser.AccessToken);
        CurrentlyPlaying targetCurrentlyPlaying = await targetClient.Player.GetCurrentlyPlaying(new());
        PlayingItem? playingItem = SpotifyHelper.GetPlayingItem(targetCurrentlyPlaying);
        if (playingItem is null)
        {
            return $"{target} is currently not listening to a song";
        }

        if (playingItem.IsLocal)
        {
            return $"can't listen to {target}'s local file";
        }

        SpotifyClient userClient = new(user.AccessToken);
        try
        {
            await userClient.Player.AddToQueue(new(playingItem.Uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            return $"no music playing on any device, you have to start your playback first";
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return $"an unknown error occurred. It might not be possible to listen to songs";
        }

        try
        {
            await userClient.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            return $"an error occurred while trying to play the song";
        }

        return $"now playing {playingItem.Title} by {string.Join(", ", playingItem.Artists)} || {playingItem.Uri}";
    }
}
