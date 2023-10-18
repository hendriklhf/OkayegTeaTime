using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class UserCache : DbCache<User>
{
    public User? this[long id] => Get(id);

    public void Add(User user)
    {
        DbController.AddUser(new(user));
        _items.AddOrSet(user.Id, user);
    }

    /// <summary>
    /// This method also accepts a username to update the username in the database if the user has changed it.
    /// </summary>
    public User? Get(long id, string? username = null)
    {
        if (!_items.TryGetValue(id, out User? user))
        {
            return null;
        }

        if (username is null || user.Username == username)
        {
            return user;
        }

        user.Username = username;
        return user;
    }

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        foreach (EntityFrameworkModels.User user in DbController.GetUsers().Where(u => _items.All(i => i.Value.Id != u.Id)))
        {
            _items.AddOrSet(user.Id, new(user));
        }

        _containsAll = true;
    }
}
