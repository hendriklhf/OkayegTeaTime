#nullable disable

using System.Collections.Immutable;

namespace OkayegTeaTime.Models.Json;

public sealed class UserLists
{
    public required long Owner { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<long> Moderators { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<long> IgnoredUsers { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<long> SecretUsers { get; init; }
}
