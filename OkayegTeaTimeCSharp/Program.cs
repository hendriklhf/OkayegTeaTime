using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Logging;
using OkayegTeaTimeCSharp.Tools.GitHub;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;
using System.Diagnostics;

namespace OkayegTeaTimeCSharp
{
    public static class Program
    {
        private static readonly bool _running = true;

        private static void Main()
        {
            Console.Title = "OkayegTeaTime";

            new JsonController().LoadData();
            new TwitchAPI().Configure();
            _ = new TwitchBot();

#if DEBUG
            ReadMeGenerator.GenerateReadMe();
#endif

            while (_running)
            {
                Console.ReadLine();
            }
        }

        public static void ConsoleOut(string value, bool logging = false, ConsoleColor fontColor = ConsoleColor.Gray)
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
            ConsoleOut($"BOT>RESTARTED", true, ConsoleColor.Red);
            Process.Start($"./OkayegTeaTimeCSharp");
            Environment.Exit(0);
        }
    }
}
