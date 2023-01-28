using System;
using System.Linq;
using System.Runtime.InteropServices;
using HLE.Collections;
using OkayegTeaTime.Database.Models;

namespace OkayegTeaTime.Database.Cache;

public sealed class UserCache : DbCache<User>
{
    public User? this[long id] => Get(id);

    public void Add(User user)
    {
        DbController.AddUser(new(user));
        _items.Add(user);
    }

    /// <summary>
    /// This method also accepts a username to update the username in the database if the user has changed it.
    /// </summary>
    public User? Get(long id, string? username = null)
    {
        GetAllItemsFromDatabase();
        User? user = null;
        Span<User> users = CollectionsMarshal.AsSpan(_items);
        for (int i = 0; i < users.Length; i++)
        {
            User u = users[i];
            if (u.Id != id)
            {
                continue;
            }

            user = u;
            break;
        }

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

    private protected override void GetAllItemsFromDatabase()
    {
        if (_containsAll)
        {
            return;
        }

        DbController.GetUsers().Where(u => _items.All(i => i.Id != u.Id)).ForEach(u => _items.Add(new(u)));
        _containsAll = true;
    }
}
