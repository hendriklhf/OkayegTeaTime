using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace OkayegTeaTime.Configuration;

public sealed class Users
{
    [Range(0, long.MaxValue)]
    public required long Owner { get; init; }

    public required ImmutableArray<long> Moderators { get; init; }

    public required ImmutableArray<long> IgnoredUsers { get; init; }
}
