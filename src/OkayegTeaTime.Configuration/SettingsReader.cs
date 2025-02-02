using System;
using System.IO;
using System.Text.Json;
using HLE.Memory;
using Microsoft.Win32.SafeHandles;

namespace OkayegTeaTime.Configuration;

public static class SettingsReader
{
    public static Settings Read(string fileName)
    {
        using SafeFileHandle handle = File.OpenHandle(fileName);
        long fileSize = RandomAccess.GetLength(handle);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fileSize, int.MaxValue);

        using PooledBufferWriter<byte> writer = new();
        int bytesRead = RandomAccess.Read(handle, writer.GetSpan((int)fileSize), 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(fileSize, bytesRead);
        writer.Advance(bytesRead);

        Settings? settings = JsonSerializer.Deserialize(writer.WrittenSpan, SettingsJsonSerializerContext.Default.Settings);
        ArgumentNullException.ThrowIfNull(settings);
        return settings;
    }
}
