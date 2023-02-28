using System.Collections.Frozen;

namespace OkayegTeaTime.Settings.Models;

public sealed class UserLists
{
    public long Owner { get; set; }

    public FrozenSet<long> Moderators { get; }

    public FrozenSet<long> IgnoredUsers { get; }

    public FrozenSet<long> SecretUsers { get; }

    public UserLists(OkayegTeaTime.Models.Json.UserLists userLists)
    {
        Moderators = userLists.Moderators.ToFrozenSet();
        IgnoredUsers = userLists.IgnoredUsers.ToFrozenSet();
        SecretUsers = userLists.SecretUsers.ToFrozenSet();
    }
}
