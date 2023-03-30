using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OkayegTeaTime.Database.EntityFrameworkModels;

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
            OkayegTeaTimeContext db = GetContext();
            try
            {
                Spotify? user = db.Spotify.FirstOrDefault(s => s.Id == Id);
                if (user is null)
                {
                    return;
                }

                user.AccessToken = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public string RefreshToken { get; }

    public long Time
    {
        get => _time;
        set
        {
            _time = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                Spotify? user = db.Spotify.FirstOrDefault(s => s.Id == Id);
                if (user is null)
                {
                    return;
                }

                user.Time = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public bool AreSongRequestsEnabled
    {
        get => _areSongRequestsEnabled;
        set
        {
            _areSongRequestsEnabled = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                Spotify? user = db.Spotify.FirstOrDefault(s => s.Id == Id);
                if (user is null)
                {
                    return;
                }

                user.SongRequestEnabled = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    private string _accessToken;
    private long _time;
    private bool _areSongRequestsEnabled;

    public SpotifyUser(Spotify spotifyUser)
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
