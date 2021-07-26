using HLE.HttpRequests;
using OkayegTeaTimeCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitchLib.Api.V5.Models.Channels;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public static class TwitchAPI
    {
        private static readonly TwitchLib.Api.TwitchAPI _api = new();

        public static void Configure()
        {
            _api.Settings.ClientId = Resources.TwitchApiClientID;
            _api.Settings.Secret = Resources.TwitchApiClientSecret;
            _api.Settings.AccessToken = GetAccessToken();
        }

        public static string GetAccessToken()
        {
            HttpPost request = new("https://id.twitch.tv/oauth2/token",
                new()
                {
                    new("client_id", _api.Settings.ClientId),
                    new("client_secret", _api.Settings.Secret),
                    new("grant_type", "client_credentials"),
                    new("scope", "channel:read:subscriptions")
                });
            return request.Data.GetProperty("access_token").GetString();
        }

        public static void RefreshAccessToken()
        {
            _api.Settings.AccessToken = GetAccessToken();
        }

        public static Channel GetChannelByName(string channel)
        {
            List<Channel> channels = _api.V5.Search.SearchChannelsAsync(HttpUtility.UrlEncode(channel), 20).Result.Channels.ToList();
            try
            {
                return channels.Where(c => c.Name == channel).FirstOrDefault();
            }
            catch (Exception)
            {
                return channels[0];
            }
        }

        public static string GetChannelID(string channel)
        {
            return GetChannelByName(channel).Id;
        }
    }
}