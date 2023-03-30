using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using HLE;
using HLE.Maths;
using HLE.Twitch;
using HLE.Memory;
using OkayegTeaTime.Database;
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct PingCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public PingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Span<char> buffer = stackalloc char[50];

        _response.Append(ChatMessage.Username, ", ");
        TimeSpan uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        uptime.TryFormat(buffer, out _, "g");
        int indexOfDot = buffer.IndexOf('.');
        _response.Append("Pingeg, I'm here! Uptime: ", buffer[..indexOfDot]);

        _response.Append(" || Ping: ");
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - ChatMessage.TmiSentTs - 5;
        _response.Append(latency);
        _response.Append("ms");

        _response.Append(" || Total process memory: ");
        _response.Append(GetProcessMemory());
        _response.Append("MB || Managed memory: ");
        _response.Append(GetManagedMemory());
        _response.Append("MB");

        if (OperatingSystem.IsLinux())
        {
            ReadOnlySpan<char> temperature = GetTemperature();
            if (temperature.Length > 0)
            {
                _response.Append(" || Temperature: ", temperature);
            }
        }

        _response.Append(" || Executed commands: ");
        _response.Append(NumberHelper.InsertKDots(_twitchBot.CommandCount));

        _response.Append(" || Running on ", RuntimeInformation.FrameworkDescription, " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetProcessMemory()
    {
        double memory = Process.GetCurrentProcess().PrivateMemorySize64;
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.Null, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    private static double GetManagedMemory()
    {
        double memory = GC.GetTotalMemory(false);
        double memoryInMegaByte = UnitPrefix.Convert(memory, UnitPrefix.Null, UnitPrefix.Mega);
        return Math.Round(memoryInMegaByte, 3);
    }

    [SupportedOSPlatform("linux")]
    private static ReadOnlySpan<char> GetTemperature()
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
            temperatureProcess.WaitForExit();
            ReadOnlySpan<char> output = temperatureProcess.StandardOutput.ReadToEnd();
            int indexOfEqualsSign = output.IndexOf('=');
            Span<char> temperature = output[(indexOfEqualsSign + 1)..].AsMutableSpan();
            temperature[4] = '°';
            return temperature;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return ReadOnlySpan<char>.Empty;
        }
    }
}
