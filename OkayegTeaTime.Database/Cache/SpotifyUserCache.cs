using System;
using System.Linq;
using OkayegTeaTime.Database.EntityFrameworkModels;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class SpotifyUserCache : DbCache<SpotifyUser>
{
    public SpotifyUser? this[string username] => Get(username);

    public void Add(string username, string accessToken, string refreshToken)
    {
        long? id = DbController.AddSpotifyUser(username, accessToken, refreshToken);
        if (!id.HasValue)
        {
            return;
        }

        SpotifyUser user = new(id.Value, username, accessToken, refreshToken);
        _items.Add(user.Id, user);
    }

    private SpotifyUser? Get(string username)
    {
        return this.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        foreach (Spotify user in DbController.GetSpotifyUsers().Where(u => _items.All(i => i.Value.Id != u.Id)))
        {
            _items.Add(user.Id, new(user));
        }

        _containsAll = true;
    }
}
