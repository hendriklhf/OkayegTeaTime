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
        private static bool _running = true;

        private static void Main()
        {
            Console.Title = "OkayegTeaTime";

            JsonController.LoadData();
            TwitchAPI.Configure();
            _ = new TwitchBot();
#if DEBUG
            ReadMeGenerator.GenerateReadMe();
#endif
            while (_running)
            {
                Consolse.ReadLine();
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
            _running = false;
            Environment.Exit(0);
        }
    }
}

#warning code needs doc
#warning move databasehelper methods
#warning discord
#warning merge of all emote actions
#warning add try-catch to sending reminder, crashes if someone spams and receives a reminder
#warning create class for channel specific messages
#warning twitter sub
#warning weather api
#warning random reddit post
#warning pick cmd
#warning .Split(int) splits in middle of a word
#warning global cooldown of cmds
#warning notify me live
#warning emote cmds for other channels
#warning "beautify" adding cooldown to dict
#warning too many static classes, make everything non-static that can be non-static
