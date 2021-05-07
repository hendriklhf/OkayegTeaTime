using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System;

namespace OkayegTeaTimeCSharp
{
    public abstract class Program
    {
        private static void Main(string[] args)
        {
            JsonHelper.SetData();
            PrefixHelper.FillDictionary();
            BotHelper.FillLastMessagesDictionary();

            TwitchBot OkayegTeaTime = new();
            OkayegTeaTime.SetBot();

            //TwitchAPI twitchAPI = new();

            Console.ReadLine();
        }
    }
}

#warning needs a list of channels in which the bot is not allowed to send massages
