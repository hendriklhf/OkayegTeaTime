using System;
using System.Collections.Generic;
using System.Linq;
using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class Config
    {
        public static string GetUsername()
        {
            return Resources.Username;
        }

        public static string GetToken()
        {
            return Resources.Token;
        }

        public static List<string> GetChannels()
        {
            return Resources.Channels.Split(" ").ToList();
        }

        public static string GetClientID()
        {
            return Resources.ClientID;
        }

        public static string GetAccessToken()
        {
            return Resources.AccesToken;
        }
    }
}
