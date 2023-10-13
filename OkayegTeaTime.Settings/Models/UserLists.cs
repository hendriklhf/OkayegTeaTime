using System.Collections.Frozen;

namespace OkayegTeaTime.Settings.Models;

public sealed class UserLists(OkayegTeaTime.Models.Json.UserLists userLists)
{
    public long Owner { get; set; } = userLists.Owner;

    public FrozenSet<long> Moderators { get; } = userLists.Moderators.ToFrozenSet();

    public FrozenSet<long> IgnoredUsers { get; } = userLists.IgnoredUsers.ToFrozenSet();

    public FrozenSet<long> SecretUsers { get; } = userLists.SecretUsers.ToFrozenSet();
}
