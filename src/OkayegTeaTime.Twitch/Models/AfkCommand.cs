using System.Diagnostics.CodeAnalysis;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Twitch.Commands;

namespace OkayegTeaTime.Twitch.Models;

[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
public sealed class AfkCommand
{
    public required AfkType Type { get; init; }

    public required string Name { get; init; }

    public required string ComingBack { get; init; }

    public required string GoingAway { get; init; }

    public required string Resuming { get; init; }

    public required string[] Aliases { get; init; }

    public required Parameter[] Parameters { get; init; }

    public required bool Document { get; init; }
}
