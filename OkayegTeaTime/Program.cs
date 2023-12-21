using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime;

public static class Program
{
    private static readonly SemaphoreSlim s_keepAliveSemaphore = new(0);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static async Task Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.UTF8;

        GlobalSettings.Initialize();

        ArgsResolver argsResolver = new(args);
        using TwitchBot twitchBot = new(argsResolver.Channels);
        await twitchBot.ConnectAsync();

        await s_keepAliveSemaphore.WaitAsync();
    }
}
