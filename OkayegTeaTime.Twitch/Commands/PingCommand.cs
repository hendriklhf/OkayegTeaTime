using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HLE;
using HLE.Maths;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public readonly unsafe ref struct PingCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string? _prefix;
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly string _alias;

    public PingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        TimeSpan uptime = DateTime.UtcNow - _twitchBot.StartTime;
        Response->Append("Pingeg, I'm here! Uptime: ", uptime.ToString("c"));

#if DEBUG
        string? temperature = GetTemperature();
        if (temperature is not null)
        {
            Response->Append(" || Temperature: ", temperature);
        }
#endif

        Span<char> latencyChars = stackalloc char[NumberHelper.GetNumberLength(_twitchBot.Latency)];
        NumberHelper.NumberToChars(_twitchBot.Latency, latencyChars);

        Response->Append(" || Memory usage: ", GetMemoryUsage().ToString(CultureInfo.InvariantCulture), "MB || Executed commands: ");
        Response->Append(NumberHelper.InsertKDots(_twitchBot.CommandCount), " || Ping: ", latencyChars);
        Response->Append("ms || Running on .NET ", Environment.Version.ToString(), " || Commit: ", ResourceController.LastCommit);
    }

    private static double GetMemoryUsage()
    {
        return Process.GetCurrentProcess().PrivateMemorySize64 / UnitPrefix.Mega;
    }

#if DEBUG
    private static string? GetTemperature()
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
            return new(temperature);
        }
        catch
        {
            return null;
        }
    }
#endif
}
