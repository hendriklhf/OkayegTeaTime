using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.DatabaseConnection;

public class Connection
{
    public string Hostname { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    [JsonIgnore]
    public string ConnectionString => $"Data Source={Hostname};DataBase={Database};User ID={Username};Password={Password};";
}
