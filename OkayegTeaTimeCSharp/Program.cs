using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Prefixes;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;

namespace OkayegTeaTimeCSharp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            JsonHelper.SetData();
            PrefixHelper.FillDictionary();
            TwitchBot OkayegTeaTime = new();
            //TwitchAPI twitchAPI = new();

            Console.ReadLine();
        }
    }
}
