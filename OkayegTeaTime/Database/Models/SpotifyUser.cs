using System.Threading.Tasks;
using HLE.Time;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Spotify;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Database.Models;

public class SpotifyUser : CacheModel
{
    public int Id { get; }

    public string Username { get; }

    public string AccessToken
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
            EntityFrameworkModels.Spotify? user = GetDbContext().Spotify.FirstOrDefault(s => s.Id == Id);
            if (user is null)
            {
                return;
            }

            user.AccessToken = value;
            EditedProperty();
        }
    }

    public string RefreshToken
    {
        get => _refreshToken;
        set
        {
            _refreshToken = value;
            EntityFrameworkModels.Spotify? user = GetDbContext().Spotify.FirstOrDefault(s => s.Id == Id);
            if (user is null)
            {
                return;
            }

            user.RefreshToken = value;
            EditedProperty();
        }
    }

    public long Time
    {
        get => _time;
        set
        {
            _time = value;
            EntityFrameworkModels.Spotify? user = GetDbContext().Spotify.FirstOrDefault(s => s.Id == Id);
            if (user is null)
            {
                return;
            }

            user.Time = value;
            EditedProperty();
        }
    }

    public bool AreSongRequestsEnabled
    {
        get => _areSongRequestsEnabled;
        set
        {
            _areSongRequestsEnabled = value;
            EntityFrameworkModels.Spotify? user = GetDbContext().Spotify.FirstOrDefault(s => s.Id == Id);
            if (user is null)
            {
                return;
            }

            user.SongRequestEnabled = value;
            EditedProperty();
        }
    }

    public string Response { get; private set; } = string.Empty;

    private string _accessToken;
    private string _refreshToken;
    private long _time;
    private bool _areSongRequestsEnabled;

    private const byte _trackIdPrefixLength = 14;

    public SpotifyUser(EntityFrameworkModels.Spotify spotifyUser)
    {
        Id = spotifyUser.Id;
        Username = spotifyUser.Username;
        _accessToken = spotifyUser.AccessToken;
        _refreshToken = spotifyUser.RefreshToken;
        _time = spotifyUser.Time;
        _areSongRequestsEnabled = spotifyUser.SongRequestEnabled == true;
    }

    public SpotifyUser(int id, string username, string accessToken, string refreshToken)
    {
        Id = id;
        Username = username;
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _time = TimeHelper.Now();
    }

    private async Task<SpotifyClient?> GetClient()
    {
        if (!IsAccessTokenExpired())
        {
            return new(AccessToken);
        }

        string? accessToken = await SpotifyController.GetNewAccessToken(Username);
        if (accessToken is null)
        {
            return null;
        }

        AccessToken = accessToken;
        Time = TimeHelper.Now();
        return new(AccessToken);
    }

    private bool IsAccessTokenExpired()
    {
        return Time + new Hour().Milliseconds <= TimeHelper.Now() + new Second(30).Milliseconds;
    }

    public async Task AddToQueue(string song)
    {
        string? uri = SpotifyController.ParseSongToUri(song);
        if (uri is null)
        {
            Response = "invalid track link";
            return;
        }

        if (!AreSongRequestsEnabled)
        {
            Response = $"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.AddToQueue(new(uri));
            FullTrack item = await client.Tracks.Get(uri[_trackIdPrefixLength..], new());
            string[] artistNames = item.Artists.Select(a => a.Name).ToArray();
            string artists = string.Join(", ", artistNames);
            Response = $"{item.Name} by {artists} || {item.Uri} has been added to the queue of {Username.Antiping()}";
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, {Username.Antiping()} probably has to start their playback first";
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to add songs to {Username.Antiping()}'s queue";
        }
    }

    public async Task Skip()
    {
        if (!AreSongRequestsEnabled)
        {
            Response = $"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.SkipNext(new());
            Response = $"skipped to next song in {Username.Antiping()}'s queue";
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to skip songs of {Username.Antiping()}'s queue";
        }
    }

    public async Task ListenTo(SpotifyUser target)
    {
        if (string.Equals(target.Username, Username, StringComparison.OrdinalIgnoreCase))
        {
            Response = "you can't listen to your own songs :)";
            return;
        }

        SpotifyItem? item = await target.GetCurrentlyPlayingItem();
        if (item is null)
        {
            Response = $"{target.Username.Antiping()} isn't listening to anything at the moment";
            return;
        }

        await ListenTo(item);
    }

    public async Task ListenTo(SpotifyItem item)
    {
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, {Username.Antiping()} probably has to start your playback first";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = "an error occurred, it might not be possible to listen to other people's songs";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occured while trying to play the song {Username.Antiping()} wanted to listen to";
            return;
        }

        if (item is SpotifyTrack track)
        {
            string artists = string.Join(", ", track.Artists.Select(a => a.Name));
            Response = $"now playing {track.Name} by {artists} || {track.Uri}";
        }
        else if (item is SpotifyEpisode episode)
        {
            Response = $"now playing {episode.Name} by {episode.Show.Name} || {episode.Uri}";
        }
        else
        {
            Response = "now listening to an unknown Spotify item type monkaS";
        }
    }

    public async Task ListenTo(string song)
    {
        string? uri = SpotifyController.ParseSongToUri(song);
        if (uri is null)
        {
            Response = "invalid track link";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.AddToQueue(new(uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, {Username.Antiping()} probably has to start your playback first";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = "an error occurred, it might not be possible to listen to other people's songs";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            await client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = "an error occured while trying to play the song you wanted to listen to";
            return;
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            FullTrack? track = await client.Tracks.Get(uri[_trackIdPrefixLength..], new());
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (track is not null)
            {
                string artists = string.Join(", ", track.Artists.Select(a => a.Name));
                Response = $"now playing {track.Name} by {artists} || {track.Uri}";
            }
            else
            {
                Response = $"now playing {uri}, but failed to retrieve further info about the playing item";
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"now playing {uri}, but failed to retrieve further info about the playing item";
        }
    }

    public async Task<SpotifyItem?> GetCurrentlyPlayingItem()
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            currentlyPlaying = await client.Player.GetCurrentlyPlaying(new());
        }
        catch (Exception ex)
        {
            Response = "an error occurred, it might not be possible to retrieve you currently playing song";
            Logger.Log(ex);
            return null;
        }

        SpotifyItem? item = null;
        // ReSharper disable once ConstantConditionalAccessQualifier
        if (currentlyPlaying?.Item is FullTrack track)
        {
            item = new SpotifyTrack(track);
        }
        else if (currentlyPlaying?.Item is FullEpisode episode)
        {
            item = new SpotifyEpisode(episode);
        }

        return item;
    }
}
