using System.Linq;
using OkayegTeaTime.Database.Cache.Enums;

namespace OkayegTeaTime.Database.Models;

public class User : CacheModel
{
    public long Id { get; }

    public string Username { get; }

    public string? AfkMessage
    {
        get => _afkMessage;
        set
        {
            _afkMessage = value;
            EntityFrameworkModels.User? efUser = DbContext.Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.AfkMessage = value;
            EditedProperty();
        }
    }

    public AfkType AfkType
    {
        get => _afkType;
        set
        {
            _afkType = value;
            EntityFrameworkModels.User? efUser = DbContext.Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.AfkType = (int)value;
            EditedProperty();
        }
    }

    public long AfkTime
    {
        get => _afkTime;
        set
        {
            _afkTime = value;
            EntityFrameworkModels.User? efUser = DbContext.Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.AfkTime = value;
            EditedProperty();
        }
    }

    public bool IsAfk
    {
        get => _isAfk;
        set
        {
            _isAfk = value;
            EntityFrameworkModels.User? efUser = DbContext.Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.IsAfk = value;
            EditedProperty();
        }
    }

    private string? _afkMessage;
    private AfkType _afkType;
    private long _afkTime;
    private bool _isAfk;

    public User(EntityFrameworkModels.User user)
    {
        Id = user.Id;
        Username = user.Username;
        _afkMessage = user.AfkMessage;
        _afkType = (AfkType)user.AfkType;
        _afkTime = user.AfkTime;
        _isAfk = user.IsAfk;
    }

    public User(long id, string username, AfkType type)
    {
        Id = id;
        Username = username;
        _afkType = type;
    }
}
