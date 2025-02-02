using System.ComponentModel.DataAnnotations;

namespace OkayegTeaTime.Configuration;

// ReSharper disable once UseNameofExpressionForPartOfTheString
public sealed class DatabaseSettings
{
    [MinLength(1)]
    public required string Hostname { get; init; }

    [MinLength(1)]
    public required string Database { get; init; }

    [MinLength(1)]
    public required string Username { get; init; }

    [MinLength(1)]
    public required string Password { get; init; }
}
