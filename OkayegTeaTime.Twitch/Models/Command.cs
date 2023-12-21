using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OkayegTeaTime.Twitch.Commands;

namespace OkayegTeaTime.Twitch.Models;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("{Name}")]
public sealed class Command
{
    public required CommandType Type { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "yes because it is immutable")]
    public required ImmutableArray<string> Aliases { get; init; }

    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "yes because it is immutable")]
    public required ImmutableArray<Parameter> Parameters { get; init; }

    public required TimeSpan Cooldown { get; init; }

    public required bool Document { get; init; }
}
