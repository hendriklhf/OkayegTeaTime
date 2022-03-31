using System.IO;
using System.Text.Json;
using OkayegTeaTime.Files.Jsons;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Files.Jsons.Settings;

#nullable disable

namespace OkayegTeaTime.Files;

public static class JsonController
{
    public static Settings Settings { get; private set; }

    public static CommandList CommandList { get; private set; }

    public static List<GachiSong> GachiSongs { get; } = JsonSerializer.Deserialize<List<GachiSong>>(FileController.GachiSongs);

    public static void Initialize()
    {
        string settingsPath = FileLocator.Find(AppSettings.SettingsFileName);
        Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsPath));
        CommandList = JsonSerializer.Deserialize<CommandList>(FileController.Commands);
    }
}
