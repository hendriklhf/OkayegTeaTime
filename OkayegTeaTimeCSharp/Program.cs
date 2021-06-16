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
            ConsoleOut("args?");
            string[] args = Console.ReadLine().Split();

            JsonHelper.SetData();
            ReadMeGenerator.GenerateReadMe();
            PrefixHelper.FillDictionary();
            BotActions.FillLastMessagesDictionary();

            TwitchAPI.Configure();

            TwitchBot OkayegTeaTime = new(args);
            OkayegTeaTime.SetBot();

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

#warning needs a list of channels in which the bot is not allowed to send messages
#warning change prefixstring column to varbinary to support emojis
#warning poopeg, pisseg
#warning code needs doc
#warning move databasehelper methods
#warning "randeg strbhlfe" always sends my first message
#warning cookieg always shows the same cookie
#warning emote cmd not available for external channels
#warning create own class library