using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Numerics;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using Microsoft.Win32.SafeHandles;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand<PingCommand>(CommandType.Ping)]
public readonly struct PingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<PingCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public ChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private const string TemperatureFilePath = "/sys/class/thermal/thermal_zone0/temp";

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out PingCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
#pragma warning disable S6354
        long unixMillisecondsNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#pragma warning restore S6354

        using Process currentProcess = Process.GetCurrentProcess();

        Response.Append(ChatMessage.Username);
        Response.Append(", ");

#pragma warning disable S6563, S6561, S6354
        TimeSpan uptime = DateTime.Now - currentProcess.StartTime;
#pragma warning restore S6354, S6561, S6563

        Response.Append("Pingeg, I'm here! Uptime: ");
        Response.Append(uptime, "g");

        Response.Append(" || Ping: ");
        long latency = unixMillisecondsNow - ChatMessage.TmiSentTs;
        Response.Append(latency);
        Response.Append("ms");

        Response.Append(" || Total process memory: ");
        Response.Append(GetProcessMemoryInMegaByte(currentProcess));
        Response.Append("MB || Managed memory: ");
        Response.Append(GetManagedMemoryInMegaByte());
        Response.Append("MB");

        if (OperatingSystem.IsLinux())
        {
            double temperature = await GetTemperatureAsync();
            if (temperature != 0)
            {
                Response.Append(" || Temperature: ");
                Response.Append(temperature);
                Response.Append("°C");
            }
        }

        Response.Append(" || Executed commands: ");
        Response.Append(_twitchBot.CommandCount, "N0");

        Response.Append(" || Running on ");
        Response.Append(RuntimeInformation.FrameworkDescription);
        Response.Append(" (");
        Response.Append(RuntimeInformation.RuntimeIdentifier);
        Response.Append(") || Commit: ");
        Response.Append(ResourceController.LastCommit);
    }

    private static double GetProcessMemoryInMegaByte(Process process)
    {
        double memory = process.PrivateMemorySize64;
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.None, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    private static double GetManagedMemoryInMegaByte()
    {
        double memory = GC.GetTotalMemory(false);
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.None, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    [SupportedOSPlatform("linux")]
    private static async ValueTask<double> GetTemperatureAsync()
    {
        try
        {
            using SafeFileHandle handle = File.OpenHandle(TemperatureFilePath);
            using PooledBufferWriter<byte> byteWriter = new(16);
            int bytesRead = await RandomAccess.ReadAsync(handle, byteWriter.GetMemory(16), 0);
            byteWriter.Advance(bytesRead);
            return double.Parse(byteWriter.WrittenSpan) / 1000;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return 0;
        }
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(PingCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is PingCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(PingCommand left, PingCommand right) => left.Equals(right);

    public static bool operator !=(PingCommand left, PingCommand right) => !left.Equals(right);
}
