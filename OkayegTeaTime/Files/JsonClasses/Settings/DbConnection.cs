#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.JsonClasses.Settings;

public class DbConnection
{
    public string Hostname { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    [JsonIgnore]
    public string ConnectionString => $"Data Source={Hostname};DataBase={Database};User ID={Username};Password={Password};";
}
