using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        _response.Append(ChatMessage.Username, ", ");
        Span<char> buffer = stackalloc char[50];
        _twitchBot.Uptime.TryFormat(buffer, out int bufferLength, "g", CultureInfo.InvariantCulture);
        int indexOfDot = buffer.IndexOf('.');
        _response.Append("Pingeg, I'm here! Uptime: ", buffer[..indexOfDot]);

        if (OperatingSystem.IsLinux())
        {
            ReadOnlySpan<char> temperature = GetTemperature();
            if (temperature.Length > 0)
            {
                _response.Append(" || Temperature: ", temperature);
            }
        }

        GetMemoryUsage().TryFormat(buffer, out bufferLength, default, CultureInfo.InvariantCulture);
        _response.Append(" || Memory usage: ", buffer[..bufferLength], "MB || Executed commands: ");

        _response.Append(NumberHelper.InsertKDots(_twitchBot.CommandCount), " || Ping: ");
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - ChatMessage.TmiSentTs - 5;
        _response.Append(latency);

        Environment.Version.TryFormat(buffer, out bufferLength);
        _response.Append("ms || Running on .NET ", buffer[..bufferLength], " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetMemoryUsage()
    {
        double memory = Process.GetCurrentProcess().PrivateMemorySize64;
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
