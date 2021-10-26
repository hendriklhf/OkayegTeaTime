using System.IO;
using System.Text.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Settings;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.JsonData;

public class JsonController
{
    public Settings Settings => _settings;

    public CommandLists CommandLists => _commandLists;

    private static Settings _settings;
    private static CommandLists _commandLists;

    public void LoadData()
    {
        _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.SettingsJson));
        _commandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Path.CommandsJson));
    }
}
