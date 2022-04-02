using System.Threading.Tasks;
using System.Timers;
using HLE.Time;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
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

    public List<SpotifyUser> ListeningUsers { get; } = new();

    private string _accessToken;
    private string _refreshToken;
    private long _time;
    private bool _areSongRequestsEnabled;

    private readonly Timer _timer = new();

    private const byte _trackIdPrefixLength = 14;

    public SpotifyUser(EntityFrameworkModels.Spotify spotifyUser)
    {
        Id = spotifyUser.Id;
        Username = spotifyUser.Username;
        _accessToken = spotifyUser.AccessToken;
        _refreshToken = spotifyUser.RefreshToken;
        _time = spotifyUser.Time;
        _areSongRequestsEnabled = spotifyUser.SongRequestEnabled == true;

        _timer.Elapsed += Timer_OnElapsed;
    }

    public SpotifyUser(int id, string username, string accessToken, string refreshToken)
    {
        Id = id;
        Username = username;
        _accessToken = accessToken;
        _refreshToken = refreshToken;
        _time = TimeHelper.Now();

        _timer.Elapsed += Timer_OnElapsed;
    }

    private async void Timer_OnElapsed(object? sender, ElapsedEventArgs e)
    {
        SpotifyItem? song;
        try
        {
            song = await GetCurrentlyPlayingItem();
        }
        catch (SpotifyException)
        {
            ListeningUsers.Clear();
            _timer.Stop();
            return;
        }

        if (song is null)
        {
            ListeningUsers.Clear();
            _timer.Stop();
            return;
        }

        _timer.Interval = song.Duration;
        _timer.Start();

        foreach (SpotifyUser user in ListeningUsers)
        {
            try
            {
                await user.ListenTo(song);
            }
            catch (SpotifyException)
            {
                ListeningUsers.Remove(user);
            }
        }
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

    public async Task<SpotifyItem> AddToQueue(string song)
    {
        string? uri = SpotifyController.ParseSongToUri(song);
        if (uri is null)
        {
            throw new SpotifyException("invalid track link");
        }

        if (!AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(uri));
            FullTrack item = await client.Tracks.Get(uri[_trackIdPrefixLength..], new());
            return new(item);
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occurred, {Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to add songs to {Username.Antiping()}'s queue");
        }
    }

    public async Task Skip()
    {
        if (!AreSongRequestsEnabled)
        {
            throw new SpotifyException($"song requests are currently not enabled, {Username.Antiping()} or a moderator has to enable it first");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext(new());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to skip songs of {Username.Antiping()}'s queue");
        }
    }

    public async Task<SpotifyItem> ListenAlongWith(SpotifyUser target)
    {
        if (string.Equals(target.Username, Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to yourself :)");
        }

        CurrentlyPlayingContext? playback = await target.GetCurrentlyPlayingContext();
        if (playback is null)
        {
            throw new SpotifyException($"{target.Username.Antiping()} isn't listening to anything at the moment");
        }

        SpotifyItem item;
        // ReSharper disable once ConstantConditionalAccessQualifier
        if (playback.Item is FullTrack track)
        {
            item = new SpotifyTrack(track);
        }
        else if (playback.Item is FullEpisode episode)
        {
            item = new SpotifyEpisode(episode);
        }
        else
        {
            item = new(playback.Item);
        }

        int seekTo = playback.ProgressMs > 500 ? playback.ProgressMs : 0;
        await ListenTo(item, seekTo);
        ListeningUsers.Clear();
        _timer.Stop();
        if (!target.ListeningUsers.Contains(this))
        {
            target.ListeningUsers.Add(this);
        }

        int interval = item.Duration - playback.ProgressMs;
        target.StartTimer(interval);
        return item;
    }

    private void StartTimer(int interval)
    {
        _timer.Stop();
        _timer.Interval = interval + 500;
        _timer.Start();
    }

    public SpotifyUser? GetListeningTo()
    {
        return DbControl.SpotifyUsers.FirstOrDefault(u => u.ListeningUsers.Contains(this) && u != this);
    }

    public async Task<SpotifyItem> ListenTo(SpotifyUser target, int seekToMs = default)
    {
        if (string.Equals(target.Username, Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new SpotifyException("you can't listen to your own songs :)");
        }

        CurrentlyPlayingContext? playback = await target.GetCurrentlyPlayingContext();
        if (playback is null)
        {
            throw new SpotifyException($"{target.Username.Antiping()} isn't listening to anything at the moment");
        }

        SpotifyItem song = new(playback.Item);
        return await ListenTo(song, seekToMs);
    }

    public async Task<SpotifyItem> ListenTo(SpotifyItem item, int seekToMs = default)
    {
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.AddToQueue(new(item.Uri));
        }
        catch (APIException ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occurred, {Username.Antiping()} probably has to start their playback first");
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            throw new SpotifyException("an error occurred, it might not be possible to listen to other people's songs");
        }

        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            await client.Player.SkipNext(new());
            if (seekToMs != default)
            {
                await client.Player.SeekTo(new(seekToMs));
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occured while trying to play the song {Username.Antiping()} wanted to listen to");
        }

        return item;
    }

    public async Task<SpotifyItem?> GetCurrentlyPlayingItem()
    {
        CurrentlyPlaying? currentlyPlaying;
        try
        {
            SpotifyClient? client = await GetClient();
            if (client is null)
            {
                throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
            }

            currentlyPlaying = await client.Player.GetCurrentlyPlaying(new());

            if (!currentlyPlaying.IsPlaying)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            throw new SpotifyException($"an error occurred, it might not be possible to retrieve {Username.Antiping()}'s currently playing song");
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

    private async Task<CurrentlyPlayingContext?> GetCurrentlyPlayingContext()
    {
        SpotifyClient? client = await GetClient();
        if (client is null)
        {
            throw new SpotifyException($"{Username.Antiping()} isn't registered, they have to register first");
        }

        CurrentlyPlayingContext playback = await client.Player.GetCurrentPlayback(new());
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (playback is null || !playback.IsPlaying)
        {
            return null;
        }

        return playback;
    }
}
