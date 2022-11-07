using System;
using System.Diagnostics;
using HLE;
using HLE.Maths;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public sealed class PingCommand : Command
{
    public PingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"Pingeg, I'm here! Uptime: {DateTime.UtcNow - _twitchBot.StartTime:c}";
        string? temp = GetTemperature();
        if (temp is not null)
        {
            Response += $" || Temperature: {temp}";
        }

        Response += $" || Memory usage: {GetMemoryUsage()}MB || Executed commands: {_twitchBot.CommandCount.InsertKDots()} " +
            $"|| Ping: {_twitchBot.Latency}ms || Running on .NET {Environment.Version} || Commit: {ResourceController.LastCommit}";
    }

    private static double GetMemoryUsage()
    {
        return Process.GetCurrentProcess().PrivateMemorySize64 / UnitPrefix.Mega;
    }

    private static string? GetTemperature()
    {
        try
        {
            Process tempProcess = new()
            {
                StartInfo = new("vcgencmd", "measure_temp")
                {
                    RedirectStandardOutput = true
                }
            };
            tempProcess.Start();
            tempProcess.WaitForExit();
            return tempProcess.StandardOutput.ReadToEnd().Split('=')[1];
        }
        catch
        {
            return null;
        }
    }
}
