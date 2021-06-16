using OkayegTeaTimeCSharp.GitHub;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;

namespace OkayegTeaTimeCSharp
{
    public static class Program
    {
        private static void Main()
        {
            Console.Title = "OkayegTeaTime";

            JsonHelper.SetData();
            ReadMeGenerator.GenerateReadMe();
            PrefixHelper.FillDictionary();
            BotActions.FillLastMessagesDictionary();

            TwitchAPI.Configure();

            new TwitchBot().SetBot();

            while (true)
            {
                Console.ReadLine();
            }
        }

        public static void ConsoleOut(string value)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString()[..8]} | {value}");
        }
    }
}

#warning change prefixstring column to varbinary to support emojis
#warning poopeg, pisseg
#warning code needs doc
#warning move databasehelper methods
#warning emote cmd not available for external channels
#warning create own class library