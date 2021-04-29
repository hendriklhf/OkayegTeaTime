using OkayegTeaTimeCSharp.Prefixes;
using OkayegTeaTimeCSharp.Twitch.Bot;
using System;

namespace OkayegTeaTimeCSharp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            PrefixHelper.FillDictionary();
            TwitchBot OkayegTeaTime = new();
            //TwitchAPI twitchAPI = new();

            Console.ReadLine();
        }
    }
}
