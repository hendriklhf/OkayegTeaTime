using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Prefixes;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;

namespace OkayegTeaTimeCSharp
{
    public abstract class Program
    {
        private static void Main(string[] args)
        {
            JsonHelper.SetData();
            PrefixHelper.FillDictionary();
            BotHelper.FillDictionary();

            TwitchBot OkayegTeaTime = new();
            //TwitchAPI twitchAPI = new();

            Console.ReadLine();
        }
    }
}
