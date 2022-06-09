using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using OkayegTeaTime.Files.Jsons;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Files.Jsons.Settings;
using OkayegTeaTime.Resources;

namespace OkayegTeaTime.Files;

public static class JsonController
{
    private static Settings? _settings;
    private static CommandList? _commandList;
    private static IEnumerable<GachiSong>? _gachiSongs;

    public static string? SettingsPath { get; set; }

    public static Settings GetSettings()
    {
        if (_settings is not null)
        {
            return _settings;
        }

        if (string.IsNullOrEmpty(SettingsPath))
        {
            SettingsPath = FileLocator.Find(AppSettings.SettingsFileName);
            Console.WriteLine($"Found Settings file at: {SettingsPath}");
        }

        _settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsPath));
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

    public static IEnumerable<GachiSong> GetGachiSongs()
    {
        if (_gachiSongs is not null)
        {
            return _gachiSongs;
        }

        _gachiSongs = JsonSerializer.Deserialize<GachiSong[]>(ResourceController.GachiSongs);
        return _gachiSongs ?? throw new ArgumentNullException(nameof(_gachiSongs));
    }
}
