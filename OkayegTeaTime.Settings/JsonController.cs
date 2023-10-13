using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Resources;

namespace OkayegTeaTime.Settings;

public static class JsonController
{
    public static OkayegTeaTime.Models.Json.Settings GetSettings()
    {
        OkayegTeaTime.Models.Json.Settings? settings = JsonSerializer.Deserialize<OkayegTeaTime.Models.Json.Settings>(File.ReadAllText(AppSettings.SettingsFileName));
        return settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public static CommandList GetCommandList()
    {
        CommandList? commandList = JsonSerializer.Deserialize<CommandList>(ResourceController.Commands);
        return commandList ?? throw new ArgumentNullException(nameof(commandList));
    }

    public static ImmutableArray<GachiSong> GetGachiSongs()
    {
        ImmutableArray<GachiSong> gachiSongs = JsonSerializer.Deserialize<ImmutableArray<GachiSong>>(ResourceController.GachiSongs);
        return gachiSongs.IsDefault ? throw new InvalidOperationException($"{nameof(gachiSongs)} is default.") : gachiSongs;
    }
}
