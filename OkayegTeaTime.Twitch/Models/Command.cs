using System.Collections.Immutable;

namespace OkayegTeaTime.Twitch.Models;

public sealed class Command
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public required ImmutableArray<string> Aliases { get; init; }

    public required ImmutableArray<string> Parameters { get; init; }

    public required ImmutableArray<string> ParameterDescriptions { get; init; }

    public required int Cooldown { get; init; }

    public required bool Document { get; init; }
}
