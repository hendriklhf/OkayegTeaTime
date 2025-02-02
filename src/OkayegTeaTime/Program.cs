using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.UTF8;

        GlobalSettings.Initialize();

        ArgsResolver argsResolver = new(args);
        using TwitchBot twitchBot = new(argsResolver.Channels);
        await twitchBot.ConnectAsync();

        await Task.Delay(Timeout.Infinite);
    }
}
