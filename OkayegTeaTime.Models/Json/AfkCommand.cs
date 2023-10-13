using System.Collections.Immutable;

namespace OkayegTeaTime.Models.Json;

public sealed class AfkCommand
{
    public required string Name { get; init; }

    public required string ComingBack { get; init; }

    public required string GoingAway { get; init; }

    public required string Resuming { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<string> Aliases { get; init; }

    public required ImmutableArray<string> Parameters { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<string> ParameterDescriptions { get; init; }

    public required bool Document { get; init; }
}
