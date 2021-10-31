using System.IO;
using System.Text.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Settings;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.JsonData;

public static class JsonController
{
    public static Settings Settings { get; private set; }

    public static CommandLists CommandLists { get; private set; }

    public static void LoadData()
    {
        Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.SettingsJson));
        CommandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Path.CommandsJson));
    }
}
