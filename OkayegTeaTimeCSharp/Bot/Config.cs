using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class Config
    {
        private static readonly Database.Models.OkayegTeaTimeContext database = new();

        public static string Username { get; private set; }

        public static string Token { get; private set; }

        public static List<string> Channels { get; private set; } = new();

        public static void GetUsername()
        {
            Username = database.Bots.Where(bot => bot.Id == 1).FirstOrDefault().Username;
        }

        public static void GetToken()
        {
            Token = database.Bots.Where(bot => bot.Id == 1).FirstOrDefault().Oauth;
        }

        public static void GetChannels()
        {
            Channels = database.Bots.Where(bot => bot.Id == 1).FirstOrDefault().Channels.Split(" ").ToList();
        }
    }
}
