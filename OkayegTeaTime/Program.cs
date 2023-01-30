using System;
using System.Text;
using System.Threading.Tasks;
using OkayegTeaTime.Twitch;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime;

public static class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.Unicode;

        ArgsResolver argsResolver = new(args);
        TwitchBot twitchBot = new(argsResolver.Channels);
        twitchBot.Connect();

        while (true)
        {
            Task.Delay(1000).Wait();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}
