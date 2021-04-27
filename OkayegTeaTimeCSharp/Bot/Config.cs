using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class Config
    {        
        private static Database.Models.OkayegTeaTimeContext database = new();

        public static string Username { get; set; } = "";

        public static string Token { get; set; } = "";

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

        }
    }
}
