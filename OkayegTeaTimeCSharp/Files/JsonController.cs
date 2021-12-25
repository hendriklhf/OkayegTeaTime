using System.IO;
using System.Text.Json;
using OkayegTeaTimeCSharp.Files.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Files.JsonClasses.DatabaseConnection;
using OkayegTeaTimeCSharp.Files.JsonClasses.Settings;

namespace OkayegTeaTimeCSharp.Files;

public static class JsonController
{
    public static Settings Settings { get; private set; }

    public static CommandList CommandList { get; private set; }

    public static Connection Connection { get; private set; } = JsonSerializer.Deserialize<Connection>(File.ReadAllText(Path.ConnectionString));

    public static List<string> RandomWords { get; private set; } = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(Path.RandomWords));

    public static void LoadJsonData()
    {
        Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Settings));
        CommandList = JsonSerializer.Deserialize<CommandList>(File.ReadAllText(Path.Commands));
    }
}
