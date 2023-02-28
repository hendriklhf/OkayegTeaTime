using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Json;

public sealed class DatabaseConnection
{
    public required string Hostname { get; init; }

    public required string Database { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }

    [JsonIgnore]
    public string ConnectionString => $"Data Source={Hostname};DataBase={Database};User ID={Username};Password={Password};";
}
