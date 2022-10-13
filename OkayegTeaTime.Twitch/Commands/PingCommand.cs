using System;
using System.Diagnostics;
using HLE;
using HLE.Maths;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public sealed class PingCommand : Command
{
    public PingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"Pingeg, I'm here! Uptime: {DateTime.UtcNow - _twitchBot.StartTime:c} || Memory usage: {GetMemoryUsage()}MB || Executed commands: {_twitchBot.CommandCount.InsertKDots()} " +
            $"|| Ping: {_twitchBot.Latency}ms || Running on .NET {Environment.Version} || Commit: {ResourceController.LastCommit} || Cached patterns: {PatternCreator.CacheSize}";
    }

    private static double GetMemoryUsage()
    {
        return Process.GetCurrentProcess().PrivateMemorySize64 / UnitPrefix.Mega;
    }
}
