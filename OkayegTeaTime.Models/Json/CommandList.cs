using System.Collections.Immutable;

namespace OkayegTeaTime.Models.Json;

public sealed class CommandList
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<Command> Commands { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public required ImmutableArray<AfkCommand> AfkCommands { get; init; }
}
