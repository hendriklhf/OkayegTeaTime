using System.Linq;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class UserCache : DbCache<User>
{
    public User? this[long id] => GetUser(id);

    public void Add(User user)
    {
        DbController.AddUser(new(user));
        _items.Add(user);
    }

    public User? GetUser(long id, string? username = null)
    {
        User? user = this.FirstOrDefault(u => u.Id == id);
        if (user is not null && (username is null || user.Username == username))
        {
            return user;
        }

        EntityFrameworkModels.User? efUser = DbController.GetUser(id, username);
        if (efUser is null)
        {
            return null;
        }

        if (user is null)
        {
            user = new(efUser);
            _items.Add(user);
            return user;
        }

        if (username is null)
        {
            return user;
        }

        user.Username = username;
        return user;
    }

    private protected override void GetAllFromDb()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetUsers().Where(u => _items.All(i => i.Id != u.Id)).ForEach(u => _items.Add(new(u)));
        _containsAll = true;
    }
}
