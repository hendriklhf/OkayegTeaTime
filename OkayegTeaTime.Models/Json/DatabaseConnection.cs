using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using HLE.Strings;

namespace OkayegTeaTime.Models.Json;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("{ConnectionString}")]
public sealed class DatabaseConnection
{
    public required string Hostname { get; init; }

    public required string Database { get; init; }

    public required string Username { get; init; }

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
