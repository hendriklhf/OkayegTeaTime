using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HLE;
using HLE.Collections;
using HLE.Maths;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public readonly unsafe ref struct PingCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")] [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")] [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    public PingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
        ReadOnlySpan<char> format = stackalloc char[] { 'g' };
        _twitchBot.Uptime.TryFormat(buffer, out int bufferLength, format, CultureInfo.InvariantCulture);
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

        long latency = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ChatMessage.TmiSentTs - 5;
        latency.TryFormat(buffer, out bufferLength);
        Response->Append(NumberHelper.InsertKDots(_twitchBot.CommandCount), " || Ping: ", buffer[..bufferLength]);

        Environment.Version.TryFormat(buffer, out bufferLength);
        Response->Append("ms || Running on .NET ", buffer[..bufferLength], " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetMemoryUsage()
    {
        double memory = Process.GetCurrentProcess().PrivateMemorySize64 / UnitPrefix.Mega;
        return Math.Round(memory, 3);
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
            ReadOnlySpan<char> temperature = output[ranges[1]];
            Span<char> tempSpan = temperature.AsMutableSpan();
            tempSpan[4] = '°';
            return temperature;
        }
        catch
        {
            return null;
        }
    }
#endif
}
