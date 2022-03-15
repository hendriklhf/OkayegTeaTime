using HLE.Strings;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Database.Models;

public class User : CacheModel
{
    public int Id { get; }

    public string Username { get; }

    public string? AfkMessage
    {
        get => _afkMessage;
        set
        {
            _afkMessage = value;
            EntityFrameworkModels.User? efUser = GetDbContext().Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.AfkMessage = value?.Encode();
            EditedProperty();
        }
    }

    public AfkCommandType AfkType
    {
        get => _afkType;
        set
        {
            _afkType = value;
            EntityFrameworkModels.User? efUser = GetDbContext().Users.FirstOrDefault(u => u.Id == Id);
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
            EntityFrameworkModels.User? efUser = GetDbContext().Users.FirstOrDefault(u => u.Id == Id);
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
            EntityFrameworkModels.User? efUser = GetDbContext().Users.FirstOrDefault(u => u.Id == Id);
            if (efUser is null)
            {
                return;
            }

            efUser.IsAfk = value;
            EditedProperty();
        }
    }

    private string? _afkMessage;
    private AfkCommandType _afkType;
    private long _afkTime;
    private bool _isAfk;

    public User(EntityFrameworkModels.User user)
    {
        Id = user.Id;
        Username = user.Username;
        _afkMessage = user.AfkMessage.Decode();
        _afkType = (AfkCommandType)user.AfkType;
        _afkTime = user.AfkTime;
        _isAfk = user.IsAfk == true;
    }

    public User(int id, string username, AfkCommandType type)
    {
        Id = id;
        Username = username;
        _afkType = type;
    }
}
