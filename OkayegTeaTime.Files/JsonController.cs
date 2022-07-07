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

    /// <summary>
    /// Can be set to use any Settings.json file. If not set the <see cref="FileLocator"/> will search automatically for a file named "Settings.json".
    /// </summary>
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
