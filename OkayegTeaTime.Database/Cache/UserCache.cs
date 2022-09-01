using System.Linq;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public class UserCache : DbCache<User>
{
    public User? this[long id] => GetUser(id);

    public void Add(User user)
    {
        DbController.AddUser(new EntityFrameworkModels.User(user));
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

        EntityFrameworkModels.User[] users = DbController.GetUsers();
        foreach (EntityFrameworkModels.User efUser in users)
        {
            if (_items.All(u => u.Id != efUser.Id))
            {
                _items.Add(new(efUser));
            }
        }

        _containsAll = true;
    }
}
