using System;
using System.Text.Json;
using HLE;
using HLE.Memory;

namespace OkayegTeaTime.Settings;

public static class SettingsReader
{
    public static Settings Read(string fileName)
    {
        BufferedFileReader fileReader = new(fileName);
        using PooledBufferWriter<byte> writer = new();
        fileReader.ReadBytes(writer);
        Settings? settings = JsonSerializer.Deserialize<Settings>(writer.WrittenSpan);
        ArgumentNullException.ThrowIfNull(settings);
        return settings;
    }
}
