using System;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime;

public static class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.UTF8;

        ArgsResolver argsResolver = new(args);
        argsResolver.Resolve();

        JsonController.SettingsPath = argsResolver.SettingsPath;

        TwitchBot twitchBot = new(argsResolver.Channels);
        twitchBot.Connect();

        while (true)
        {
            Task.Delay(1000).Wait();
        }
    }
}
