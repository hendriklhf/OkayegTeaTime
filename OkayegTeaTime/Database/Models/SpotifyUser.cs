using HLE.Time;

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

    private string _accessToken;
    private string _refreshToken;
    private long _time;
    private bool _areSongRequestsEnabled;

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
        _time = TimeHelper.Now() - 1000;
    }
}
