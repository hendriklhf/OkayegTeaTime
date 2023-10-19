using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Numerics;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping, typeof(PingCommand))]
public readonly struct PingCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<PingCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out PingCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        long unixMillisecondsNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        using Process currentProcess = Process.GetCurrentProcess();

        Response.Append(ChatMessage.Username, ", ");
        TimeSpan uptime = DateTime.Now - currentProcess.StartTime;
        Response.Append("Pingeg, I'm here! Uptime: ");
        Response.Append(uptime, "g");

        Response.Append(" || Ping: ");
        long latency = unixMillisecondsNow - ChatMessage.TmiSentTs;
        Response.Append(latency);
        Response.Append("ms");

        Response.Append(" || Total process memory: ");
        Response.Append(GetProcessMemory(currentProcess));
        Response.Append("MB || Managed memory: ");
        Response.Append(GetManagedMemory());
        Response.Append("MB");

        if (OperatingSystem.IsLinux())
        {
            using PooledBufferWriter<char> outputWriter = new(16);
            ReadOnlyMemory<char> temperature = await GetTemperature(outputWriter);
            if (temperature.Length > 0)
            {
                Response.Append(" || Temperature: ", temperature.Span);
            }
        }

        Response.Append(" || Executed commands: ");
        Response.Append(_twitchBot.CommandCount, "N0");

        Response.Append(" || Running on ", RuntimeInformation.FrameworkDescription, " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetProcessMemory(Process process)
    {
        double memory = process.PrivateMemorySize64;
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.None, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    private static double GetManagedMemory()
    {
        double memory = GC.GetTotalMemory(false);
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.None, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    [SupportedOSPlatform("linux")]
    private static async ValueTask<ReadOnlyMemory<char>> GetTemperature(PooledBufferWriter<char> outputWriter)
    {
        try
        {
            using Process temperatureProcess = new()
            {
                StartInfo = new("vcgencmd", "measure_temp")
                {
                    RedirectStandardOutput = true
                }
            };

            temperatureProcess.Start();
            await temperatureProcess.WaitForExitAsync();
            await ReadAllCharsAsync(temperatureProcess.StandardOutput, outputWriter);

            int indexOfEqualsSign = outputWriter.WrittenSpan.IndexOf('=');
            Memory<char> temperature = outputWriter.WrittenMemory[(indexOfEqualsSign + 1)..];
            temperature.Span[4] = '°';
            return temperature;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return ReadOnlyMemory<char>.Empty;
        }
    }

    private static async ValueTask ReadAllCharsAsync(StreamReader streamReader, PooledBufferWriter<char> charWriter)
    {
        int readChars;
        do
        {
            readChars = await streamReader.ReadAsync(charWriter.GetMemory(16));
        } while (readChars > 0);
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
