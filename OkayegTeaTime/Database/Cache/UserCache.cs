using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Database.Cache;

public class UserCache : DbCache<User>
{
    public User? this[int id] => GetUser(id);

    public void Add(int id, string username, AfkCommandType type)
    {
        User user = new(id, username, type);
        DbController.AddUser(id, username, type);
        _items.Add(user);
    }

    public User? GetUser(int id, string? username = null)
    {
        User? user = _items.FirstOrDefault(u => u.Id == id);
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
        List<EntityFrameworkModels.User> users = DbController.GetUsers();
        users.ForEach(uu =>
        {
            if (_items.All(u => u.Id != uu.Id))
            {
                _items.Add(new(uu));
            }
        });
        _containsAll = true;
    }

    public override IEnumerator<User> GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
