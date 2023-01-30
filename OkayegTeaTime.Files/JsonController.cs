using System;
using System.IO;
using System.Text.Json;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Resources;

namespace OkayegTeaTime.Files;

public static class JsonController
{
    private static Settings? _settings;
    private static CommandList? _commandList;
    private static GachiSong[]? _gachiSongs;

    public static Settings GetSettings()
    {
        if (_settings is not null)
        {
            return _settings;
        }

        _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(AppSettings.SettingsFileName));
        return _settings ?? throw new ArgumentNullException(nameof(_settings));
    }

    public static CommandList GetCommandList()
    {
        if (_commandList is not null)
        {
            return _commandList;
        }

        _commandList = JsonSerializer.Deserialize<CommandList>(ResourceController.Commands);
        return _commandList ?? throw new ArgumentNullException(nameof(_commandList));
    }

    public static GachiSong[] GetGachiSongs()
    {
        if (_gachiSongs is not null)
        {
            return _gachiSongs;
        }

        _gachiSongs = JsonSerializer.Deserialize<GachiSong[]>(ResourceController.GachiSongs);
        return _gachiSongs ?? throw new ArgumentNullException(nameof(_gachiSongs));
    }
}
