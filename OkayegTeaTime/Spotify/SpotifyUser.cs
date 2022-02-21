using System.Threading.Tasks;
using OkayegTeaTime.Logging;
using SpotifyAPI.Web;
using SpotifyModel = OkayegTeaTime.Database.Models.Spotify;

namespace OkayegTeaTime.Spotify;

public class SpotifyUser
{
    public string Username { get; private set; }

    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }

    public bool AreSongRequestsEnabled { get; }

    /// <summary>
    /// A response for the last request made to the Spotify API that can be used in chat.
    /// </summary>
    public string? Response { get; private set; }

    private readonly SpotifyClient _client;

    /// <summary>
    /// The length of "spotify:track:".
    /// </summary>
    private const byte _trackIdPrefixLength = 14;

    /// <summary>
    /// The length of "spotify:episode:".
    /// </summary>
    private const byte _episodeIdPrefixLength = 16;

    public SpotifyUser(string username, string accessToken, string refreshToken, bool areSongRequestsEnabled)
    {
        Username = username;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AreSongRequestsEnabled = areSongRequestsEnabled;
        _client = new(AccessToken);
    }

    public SpotifyUser(SpotifyModel spotifyModel)
        : this(spotifyModel.Username, spotifyModel.AccessToken, spotifyModel.RefreshToken, spotifyModel.SongRequestEnabled == true)
    {
    }

    public async Task AddToQueue(string song)
    {
        string? uri = SpotifyController.ParseSongToUri(song);
        if (uri is null)
        {
            Response = $"invalid track link";
            return;
        }

        if (!AreSongRequestsEnabled)
        {
            Response = $"song requests are currently not enabled, {Username.Antiping()} or a moderator have to enable it first";
            return;
        }

        try
        {
            await _client.Player.AddToQueue(new(uri));
            FullTrack item = await _client.Tracks.Get(uri[_trackIdPrefixLength..], new());
            string[] artistNames = item.Artists.Select(a => a.Name).ToArray();
            string artists = string.Join(", ", artistNames);
            Response = $"{item.Name} by {artists} || {item.Uri} has been added to the queue of {Username.Antiping()}";
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, you probably have to start your playback first";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to add songs to {Username.Antiping()}'s queue";
            return;
        }
        return;
    }

    public async Task Skip()
    {
        if (!AreSongRequestsEnabled)
        {
            Response = $"song requests are currently not enabled, {Username.Antiping()} or a moderator have to enable it first";
            return;
        }

        try
        {
            await _client.Player.SkipNext(new());
            Response = $"skipped to next song in queue";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to skip songs of {Username.Antiping()}'s queue";
            return;
        }
    }

    public async Task ListenTo(SpotifyUser target)
    {
        if (Equals(target))
        {
            Response = $"you can't listen to your own songs :)";
            return;
        }

        SpotifyItem? item = await target.GetCurrentlyPlayingItem();
        if (item is null)
        {
            Response = $"{target.Username} isn't listening to anything at the moment";
            return;
        }

        await ListenTo(item);
    }

    public async Task ListenTo(SpotifyItem item)
    {
        try
        {
            await _client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, you probably have to start your playback first";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to listen to other people's songs";
            return;
        }

        try
        {
            await _client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occured while trying to play the song you wanted to listen to";
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
            Response = $"invalid track link";
            return;
        }

        try
        {
            await _client.Player.AddToQueue(new(uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, you probably have to start your playback first";
            return;
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occurred, it might not be possible to listen to other people's songs";
            return;
        }

        try
        {
            await _client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            Response = $"an error occured while trying to play the song you wanted to listen to";
            return;
        }

        try
        {
            FullTrack? track = await _client.Tracks.Get(uri[_trackIdPrefixLength..], new());
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
            return;
        }
    }

    public async Task<SpotifyItem?> GetCurrentlyPlayingItem()
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            currentlyPlaying = await _client.Player.GetCurrentlyPlaying(new());
        }
        catch (Exception ex)
        {
            Response = $"an error occurred, it might not be possible to retrieve you currently playing song";
            Logger.Log(ex);
            return null;
        }

        SpotifyItem? item = null;
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

    public override bool Equals(object? obj)
    {
        return obj is SpotifyUser user && user.Username.ToLower() == Username.ToLower();
    }
}
