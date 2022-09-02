using System;
using System.Diagnostics;
using HLE;
using HLE.Time;
using OkayegTeaTime.Resources;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Ping)]
public class PingCommand : Command
{
    public PingCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Response = $"Pingeg, I'm here! Uptime: {DateTime.Now - _twitchBot.StartTime:c} || Memory usage: {GetMemoryUsage()}MB || Executed commands: {_twitchBot.CommandCount.InsertKDots()} " +
                   $"|| Ping: {TimeHelper.Now() - ChatMessage.TmiSentTs}ms || Running on .NET {Environment.Version} || Commit: {ResourceController.LastCommit}";
    }

    private static double GetMemoryUsage()
    {
        return Math.Truncate(Process.GetCurrentProcess().PrivateMemorySize64 / Math.Pow(10, 6) * 100) / 100;
    }
}
