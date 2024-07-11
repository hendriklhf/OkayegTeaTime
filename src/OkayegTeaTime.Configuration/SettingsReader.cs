using System;
using System.Text.Json;
using HLE;
using HLE.Memory;

namespace OkayegTeaTime.Configuration;

public static class SettingsReader
{
    public static Settings Read(string fileName)
    {
        BufferedFileReader fileReader = new(fileName);
        using PooledBufferWriter<byte> writer = new();
        fileReader.ReadBytes(writer);
        Settings? settings = JsonSerializer.Deserialize(writer.WrittenSpan, SettingsJsonSerializerContext.Default.Settings);
        ArgumentNullException.ThrowIfNull(settings);
        return settings;
    }
}
