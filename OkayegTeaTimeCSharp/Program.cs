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
    }
}

#warning needs a list of channels in which the bot is not allowed to send messages
#warning change prefixstring column to varbinary to support emojis
#warning poopeg, pisseg
#warning code needs doc
#warning move databasehelper methods
#warning "randeg strbhlfe" always sends my first message
#warning remove "(no message)" from afk messages
#warning twitch api access token needs to be renewed after 60days
#warning cookieg always shows the same cookie
#warning too long messages wont be send
#warning doesnt reconnect on disconnect
#warning "DansGame" wont set isafk to false
#warning remove own name from massping
#warning make AfkMessage's ctor public and stop using the create method
#warning sends message multiple times if spamming