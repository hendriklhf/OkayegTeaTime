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
        Owner = userLists.Owner;
        Moderators = userLists.Moderators.ToFrozenSet(true);
        IgnoredUsers = userLists.IgnoredUsers.ToFrozenSet(true);
        SecretUsers = userLists.SecretUsers.ToFrozenSet(true);
    }
}
