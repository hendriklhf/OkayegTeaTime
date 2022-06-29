using HLE.Time;

#nullable disable

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string AfkMessage { get; set; }
    public int AfkType { get; set; }
    public long AfkTime { get; set; } = TimeHelper.Now();
    public bool IsAfk { get; set; }
    public string Location { get; set; }
    public bool IsPrivateLocation { get; set; }

    public User(long id, string username, string afkMessage, int afkType, long afkTime, bool isAfk)
    {
        Id = id;
        Username = username;
        AfkMessage = afkMessage;
        AfkType = afkType;
        AfkTime = afkTime;
        IsAfk = isAfk;
    }

    public User(long id, string username)
    {
        Id = id;
        Username = username;
    }

    public User(Models.User user)
    {
        Id = user.Id;
        Username = user.Username;
        AfkMessage = user.AfkMessage;
        AfkType = (int)user.AfkType;
        AfkTime = user.AfkTime;
        IsAfk = user.IsAfk;
        Location = user.Location;
        IsPrivateLocation = user.IsPrivateLocation;
    }
}
