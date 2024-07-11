using System;
using System.Diagnostics.CodeAnalysis;

namespace OkayegTeaTime.Database.EntityFrameworkModels;

public sealed class User
{
    public long Id { get; set; }

    public string Username { get; set; }

    public string? AfkMessage { get; set; }

    public int AfkType { get; set; }

    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public long AfkTime { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public bool IsAfk { get; set; }

    public string? Location { get; set; }

    public bool IsPrivateLocation { get; set; }

    public double UtcOffset { get; set; }

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
        UtcOffset = user.UtcOffset;
    }
}
