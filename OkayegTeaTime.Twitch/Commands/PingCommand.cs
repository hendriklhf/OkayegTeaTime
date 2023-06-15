using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using HLE.Memory;
using HLE.Numerics;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping, typeof(PingCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct PingCommand : IChatCommand<PingCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public PingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out PingCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
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
            ReadOnlyMemory<char> temperature = await GetTemperature();
            if (temperature.Length > 0)
            {
                Response.Append(" || Temperature: ", temperature.Span);
            }
        }

        Response.Append(" || Executed commands: ");
        Response.Advance(NumberHelper.InsertThousandSeparators(_twitchBot.CommandCount, '.', Response.FreeBufferSpan));

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
    private static async ValueTask<ReadOnlyMemory<char>> GetTemperature()
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
            string output = await temperatureProcess.StandardOutput.ReadToEndAsync();
            int indexOfEqualsSign = output.IndexOf('=');
            Memory<char> temperature = output.AsMemory(indexOfEqualsSign + 1).AsMutableMemory();
            temperature.Span[4] = '°';
            return temperature;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return ReadOnlyMemory<char>.Empty;
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
