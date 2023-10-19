using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using HLE.Strings;

namespace OkayegTeaTime.Settings;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("{ConnectionString}")]
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

    [JsonIgnore]
    public string ConnectionString => _connectionString ??= BuildConnectionString();

    private string? _connectionString;

    [SkipLocalsInit]
    private string BuildConnectionString()
    {
        ValueStringBuilder builder = new(stackalloc char[512]);
        builder.Append("Data Source=", Hostname, ";DataBase=", Database);
        builder.Append(";User ID=", Username, ";Password=", Password);
        return builder.ToString();
    }
}
