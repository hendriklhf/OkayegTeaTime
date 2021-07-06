using OkayegTeaTimeCSharp.GitHub;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;
using System.IO;

namespace OkayegTeaTimeCSharp
{
    public static class Program
    {
        private static void Main()
        {
            Console.Title = "OkayegTeaTime";

            TwitchAPI.Configure();
            new TwitchBot().SetBot();

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "readme")
                {
                    ReadMeGenerator.GenerateReadMe();
                }
            }
        }

        public static void ConsoleOut(string value, bool logging = false, ConsoleColor fontColor = ConsoleColor.Gray)
        {
            Console.ForegroundColor = fontColor;
            Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString()[..8]} | {value}");
            Console.ForegroundColor = ConsoleColor.Gray;
            if (logging)
            {
                File.AppendAllText(Resources.LogsPath, $"{DateTime.Today:dd/MM/yyyy HH:mm:ss} | {value}\n");
            }
        }
    }
}

#warning code needs doc
#warning move databasehelper methods
#warning discord
#warning merge of all emote actions
#warning add try-catch to sending reminder, crashes if someone spams and receives a reminder