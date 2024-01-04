using System.Linq;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.EntityFrameworkModels;

namespace OkayegTeaTime.Database.Models;

public sealed class User(long id, string username) : CacheModel
{
    public long Id { get; } = id;

    public string Username { internal set; get; } = username;

    public string? AfkMessage
    {
        get => _afkMessage;
        set
        {
            _afkMessage = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.AfkMessage = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public AfkType AfkType
    {
        get => _afkType;
        set
        {
            _afkType = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.AfkType = (int)value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public long AfkTime
    {
        get => _afkTime;
        set
        {
            _afkTime = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.AfkTime = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public bool IsAfk
    {
        get => _isAfk;
        set
        {
            _isAfk = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.IsAfk = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public string? Location
    {
        get => _location;
        set
        {
            _location = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.Location = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public bool IsPrivateLocation
    {
        get => _isPrivateLocation;
        set
        {
            _isPrivateLocation = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.IsPrivateLocation = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    public double UtcOffset
    {
        get => _utcOffset;
        set
        {
            _utcOffset = value;
            OkayegTeaTimeContext db = GetContext();
            try
            {
                EntityFrameworkModels.User? efUser = db.Users.FirstOrDefault(u => u.Id == Id);
                if (efUser is null)
                {
                    return;
                }

                efUser.UtcOffset = value;
                EditedProperty();
            }
            finally
            {
                ReturnContext();
            }
        }
    }

    private string? _afkMessage;
    private AfkType _afkType;
    private long _afkTime;
    private bool _isAfk;
    private string? _location;
    private bool _isPrivateLocation;
    private double _utcOffset;

    public User(EntityFrameworkModels.User user) : this(user.Id, user.Username)
    {
        _afkMessage = user.AfkMessage;
        _afkType = (AfkType)user.AfkType;
        _afkTime = user.AfkTime;
        _isAfk = user.IsAfk;
        _location = user.Location;
        _isPrivateLocation = user.IsPrivateLocation;
        _utcOffset = user.UtcOffset;
    }
}
