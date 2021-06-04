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
            Console.WriteLine("args?");
            string[] args = Console.ReadLine().Split(" ");

            JsonHelper.SetData();
            ReadMeGenerator.GenerateReadMe();
            PrefixHelper.FillDictionary();
            BotActions.FillLastMessagesDictionary();

            TwitchAPI.Configure();

            TwitchBot OkayegTeaTime = new(args);
            OkayegTeaTime.SetBot();

            Console.ReadLine();
        }
    }
}

#warning needs a list of channels in which the bot is not allowed to send messages
#warning cmd type const missing in some cmd classes
#warning change prefixstring column to varbinary to support emojis
#warning poopeg, pisseg
#warning code needs doc
#warning move databasehelper methods
#warning nukes wont be deleted
#warning "randeg strbhlfe" always sends my first message
#warning remove "(no message)" from afk messages
#warning twitch api access token needs to be renewed after 60days