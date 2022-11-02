using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OkayegTeaTime.Database.Models;

[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public sealed class SpotifyUser : CacheModel
{
    public long Id { get; }

    public string Username { get; }

    public string AccessToken
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.AccessToken = value;
            EditedProperty();
        }
    }

    public string RefreshToken { get; }

    public long Time
    {
        get => _time;
        set
        {
            _time = value;
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
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
            _mutex.WaitOne();
            EntityFrameworkModels.Spotify? user = DbContext.Spotify.FirstOrDefault(s => s.Id == Id);
            _mutex.ReleaseMutex();
            if (user is null)
            {
                return;
            }

            user.SongRequestEnabled = value;
            EditedProperty();
        }
    }

    private string _accessToken;
    private long _time;
    private bool _areSongRequestsEnabled;

    public SpotifyUser(EntityFrameworkModels.Spotify spotifyUser)
    {
        Id = spotifyUser.Id;
        Username = spotifyUser.Username;
        _accessToken = spotifyUser.AccessToken;
        RefreshToken = spotifyUser.RefreshToken;
        _time = spotifyUser.Time;
        _areSongRequestsEnabled = spotifyUser.SongRequestEnabled;
    }

    public SpotifyUser(long id, string username, string accessToken, string refreshToken)
    {
        Id = id;
        Username = username;
        _accessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
