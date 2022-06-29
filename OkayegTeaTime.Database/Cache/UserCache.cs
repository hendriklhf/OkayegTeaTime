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
        if (user is not null)
        {
            return user;
        }

        EntityFrameworkModels.User? efUser = DbController.GetUser(id, username);
        if (efUser is null)
        {
            return null;
        }

        user = new(efUser);
        _items.Add(user);
        return user;
    }

    private protected override void GetAllFromDb()
    {
        if (_containsAll)
        {
            return;
        }

        EntityFrameworkModels.User[] users = DbController.GetUsers();
        foreach (EntityFrameworkModels.User uu in users)
        {
            if (_items.All(u => u.Id != uu.Id))
            {
                _items.Add(new(uu));
            }
        }

        _containsAll = true;
    }
}
