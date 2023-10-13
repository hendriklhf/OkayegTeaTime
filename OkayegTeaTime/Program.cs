using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime;

public static class Program
{
    private static readonly SemaphoreSlim _keepAliveSemaphore = new(0);

    private static async Task Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.UTF8;

        AppSettings.Initialize();

        ArgsResolver argsResolver = new(args);
        using TwitchBot twitchBot = new(argsResolver.Channels);
        await twitchBot.ConnectAsync();

        await _keepAliveSemaphore.WaitAsync();
    }
}
