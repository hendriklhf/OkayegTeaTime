using System;
#if RELEASE
using System.Collections.Generic;
#endif
using System.Linq;
#if RELEASE
using System.Threading.Tasks;
#endif
using HLE.Collections;
using OkayegTeaTime.Database.Models;
#if RELEASE
using OkayegTeaTime.Files;
using OkayegTeaTime.Spotify;
#endif

namespace OkayegTeaTime.Database.Cache;

public sealed class SpotifyUserCache : DbCache<SpotifyUser>
{
    public SpotifyUser? this[string username] => GetSpotifyUser(username);

#if RELEASE
    public static List<string> ChatPlaylistUris
    {
        get
        {
            if (_chatPlaylistUris is not null)
            {
                return _chatPlaylistUris;
            }

            static async Task GetPlaylistTracks()
            {
                string? username = DbController.GetUser(AppSettings.UserLists.Owner)?.Username;
                if (username is null)
                {
                    return;
                }

                EntityFrameworkModels.Spotify? efUser = DbController.GetSpotifyUser(username);
                if (efUser is null)
                {
                    return;
                }

                SpotifyUser user = new(efUser);
                IEnumerable<SpotifyTrack> tracks = await user.GetPlaylistItems(AppSettings.Spotify.ChatPlaylistId);
                _chatPlaylistUris = tracks.Select(t => t.Uri).ToList();
            }

            GetPlaylistTracks().Wait();
            _chatPlaylistUris ??= new();
            return _chatPlaylistUris;
        }
    }

    private static List<string>? _chatPlaylistUris;
#endif

    public void Add(string username, string accessToken, string refreshToken)
    {
        long? id = DbController.AddSpotifyUser(username, accessToken, refreshToken);
        if (!id.HasValue)
        {
            return;
        }

        SpotifyUser user = new(id.Value, username, accessToken, refreshToken);
        _items.Add(user);
    }

    private SpotifyUser? GetSpotifyUser(string username)
    {
        SpotifyUser? user = this.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        if (user is not null)
        {
            return user;
        }

        EntityFrameworkModels.Spotify? efUser = DbController.GetSpotifyUser(username);
        if (efUser is null)
        {
            return null;
        }

        user = new(efUser);
        _items.Add(user);
        return user;
    }

    /// <summary>
    ///     Returns who the <paramref name="user" /> is listening to. Null, if they are not listening to anyone.
    /// </summary>
    public SpotifyUser? GetListeningTo(SpotifyUser user)
    {
        return this.FirstOrDefault(u => u.ListeningUsers.Contains(user) && u != user);
    }

    private protected override void GetAllFromDb()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetSpotifyUsers().Where(u => _items.All(i => i.Id != u.Id)).ForEach(u => _items.Add(new(u)));
        _containsAll = true;
    }
}
