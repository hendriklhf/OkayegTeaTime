using System.Diagnostics;
using System.Text;
using System.Threading;
using OkayegTeaTime.Files;
using OkayegTeaTime.Logging;
using OkayegTeaTime.Twitch.Bot;

namespace OkayegTeaTime;

public static class Program
{
    private static bool _running = true;

    private static void Main(string[] args)
    {
        Console.Title = "OkayegTeaTime";
        Console.OutputEncoding = Encoding.UTF8;

        JsonController.Initialize();

        TwitchBot twitchBot = args.Any() ? new(args) : new();
        twitchBot.Connect();

        while (_running)
        {
            Thread.Sleep(1000);
        }
    }

    public static void ConsoleOut(string value, ConsoleColor fontColor = ConsoleColor.Gray, bool logging = false)
    {
        Console.ForegroundColor = fontColor;
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {value}");
        Console.ForegroundColor = ConsoleColor.Gray;
        if (logging)
        {
            Logger.Log(value);
        }
    }

    public static void DebugOut(string value, bool logging = false)
    {
        Debug.WriteLine($"{DateTime.Now:HH:mm:ss} | {value}");
        if (logging)
        {
            Logger.Log(value);
        }
    }

    public static void Restart()
    {
        ConsoleOut($"[SYSTEM] RESTARTED", ConsoleColor.Red, true);
        Process.Start($"./{AppSettings.AssemblyName}");
        _running = false;
        Environment.Exit(0);
    }
}
