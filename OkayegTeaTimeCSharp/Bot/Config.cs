using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.Bot
{
    public static class Config
    {
        public static string Username { get; set; } = "";

        public static string Token { get; set; } = "";

        public static void GetUsername()
        {
            Username = database.Bots.Where(bot => bot.Id == 1).FirstOrDefault().Username;
        }

        public static void GetToken()
        {
            Token = database.Bots.Where(bot => bot.Id == 1).FirstOrDefault().Oauth;
        }

        public static List<string> Channels { get; private set; } = new()
        {
            "okayegteatime",
            "strbhlfe",
            "xxdirkthecrafterxx",
            "derpalt",
            "moondye7",
            "ronic76",
            "odin_eu",
            "winnie_po",
            "benastro",
            "jonas5477",
            "enno_of",
            "jann_amh_",
            "timix2g",
            "jonasenbluten",
            "w201diesel"
        };

        private static Database.Models.OkayegTeaTimeContext database = new();
    }
}
