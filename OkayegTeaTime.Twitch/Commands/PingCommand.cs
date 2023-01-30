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
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
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
        Response->Append("Pingeg, I'm here! Uptime: ", _twitchBot.Uptime.ToString("c"));

#if DEBUG
        ReadOnlySpan<char> temperature = GetTemperature();
        if (temperature.Length > 0)
        {
            Response->Append(" || Temperature: ", temperature);
        }
#endif

        Span<char> latencyChars = stackalloc char[30];
        _twitchBot.Latency.TryFormat(latencyChars, out int latencyLength);
        latencyChars = latencyChars[..latencyLength];

        Response->Append(" || Memory usage: ", GetMemoryUsage().ToString(CultureInfo.InvariantCulture), "MB || Executed commands: ");
        Response->Append(NumberHelper.InsertKDots(_twitchBot.CommandCount), " || Ping: ", latencyChars);
        Response->Append("ms || Running on .NET ", Environment.Version.ToString(), " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetMemoryUsage()
    {
        return Process.GetCurrentProcess().PrivateMemorySize64 / UnitPrefix.Mega;
    }

#if DEBUG
    private static ReadOnlySpan<char> GetTemperature()
    {
        try
        {
            Process temperatureProcess = new()
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
