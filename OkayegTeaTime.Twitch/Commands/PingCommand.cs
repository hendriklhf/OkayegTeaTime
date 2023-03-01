using System;
using System.Diagnostics;
using System.Globalization;
using HLE;
using HLE.Maths;
using HLE.Twitch;
#if DEBUG
using HLE.Memory;
#endif
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public readonly ref struct PingCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    // ReSharper disable once NotAccessedField.Local
    private readonly string? _prefix;
    // ReSharper disable once NotAccessedField.Local
    private readonly string _alias;

    private const string _uptimeFormat = "g";

    public PingCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
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
        _twitchBot.Uptime.TryFormat(buffer, out int bufferLength, _uptimeFormat, CultureInfo.InvariantCulture);
        _response.Append("Pingeg, I'm here! Uptime: ", buffer[..bufferLength]);

#if DEBUG
        ReadOnlySpan<char> temperature = GetTemperature();
        if (temperature.Length > 0)
        {
            _response.Append(" || Temperature: ", temperature);
        }
#endif

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

#if DEBUG
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
            Span<Range> ranges = stackalloc Range[output.Length];
            output.GetRangesOfSplit('=', ranges);
            Span<char> temperature = output[ranges[1]].AsMutableSpan();
            temperature[4] = '°';
            return temperature;
        }
        catch
        {
            return null;
        }
    }
#endif
}
