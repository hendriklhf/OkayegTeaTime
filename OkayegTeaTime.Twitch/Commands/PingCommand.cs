using System;
using System.Diagnostics;
using System.Globalization;
using HLE;
using HLE.Maths;
#if DEBUG
using HLE.Memory;
#endif
using HLE.Twitch.Models;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public readonly unsafe ref struct PingCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    // ReSharper disable once NotAccessedField.Local
    private readonly string? _prefix;
    // ReSharper disable once NotAccessedField.Local
    private readonly string _alias;

    private const string _uptimeFormat = "g";

    public PingCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Response->Append(ChatMessage.Username, Messages.CommaSpace);
        Span<char> buffer = stackalloc char[50];
        _twitchBot.Uptime.TryFormat(buffer, out int bufferLength, _uptimeFormat, CultureInfo.InvariantCulture);
        Response->Append("Pingeg, I'm here! Uptime: ", buffer[..bufferLength]);

#if DEBUG
        ReadOnlySpan<char> temperature = GetTemperature();
        if (temperature.Length > 0)
        {
            Response->Append(" || Temperature: ", temperature);
        }
#endif

        GetMemoryUsage().TryFormat(buffer, out bufferLength, default, CultureInfo.InvariantCulture);
        Response->Append(" || Memory usage: ", buffer[..bufferLength], "MB || Executed commands: ");

        Response->Append(NumberHelper.InsertKDots(_twitchBot.CommandCount), " || Ping: ");
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long latency = now - ChatMessage.TmiSentTs - 5;
        Response->Append(latency);

        Environment.Version.TryFormat(buffer, out bufferLength);
        Response->Append("ms || Running on .NET ", buffer[..bufferLength], " || Commit: ", ResourceController.LastCommit);
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
