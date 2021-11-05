﻿using System.IO;
using System.Text.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.DatabaseConnection;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Settings;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.JsonData;

public static class JsonController
{
    public static Settings Settings { get; private set; }

    public static CommandLists CommandLists { get; private set; }

    public static Connection Connection { get; private set; } = JsonSerializer.Deserialize<Connection>(File.ReadAllText(Path.ConnectionString));

    public static List<string> RandomWords { get; private set; } = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(Path.RandomWords));

    public static void LoadData()
    {
        Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.SettingsJson));
        CommandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Path.CommandsJson));
    }
}
