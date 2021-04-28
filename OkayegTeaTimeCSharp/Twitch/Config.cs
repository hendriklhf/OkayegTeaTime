using System;
using System.Collections.Generic;
using System.Linq;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class Config
    {
        private static readonly Database.Models.OkayegTeaTimeContext database = new();

        public static string Username { get; private set; }

        public static string Token { get; private set; }

        public static List<string> Channels { get; private set; } = new();

        public static string ClientID { get; private set; }

        public static string AccessToken { get; private set; }

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

        public static void GetClientID()
        {
            throw new NotImplementedException();
        }

        public static void GetAccessToken()
        {
            throw new NotImplementedException();
        }
    }
}
