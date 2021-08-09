using HLE.HttpRequests;
using OkayegTeaTimeCSharp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.V5.Models.Channels;
using static OkayegTeaTimeCSharp.Program;
using TwitchLibAPI = TwitchLib.Api.TwitchAPI;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public static class TwitchAPI
    {
        private static readonly TwitchLibAPI _api = new();
        private static LiveStreamMonitorService _monitor;
        private static readonly List<string> _monitorChannels; // = Database.GetMonitorChannels();

        public static void Configure()
        {
            _api.Settings.ClientId = Resources.TwitchApiClientID;
            _api.Settings.Secret = Resources.TwitchApiClientSecret;
            _api.Settings.AccessToken = GetAccessToken();
            ConfigureLiveMonitor();
        }

        public static string GetAccessToken()
        {
            HttpPost request = new("https://id.twitch.tv/oauth2/token",
                new()
                {
                    new("client_id", _api.Settings.ClientId),
                    new("client_secret", _api.Settings.Secret),
                    new("grant_type", "client_credentials"),
                    new("scope", "user_subscriptions")
                });
            return request.Data.GetProperty("access_token").GetString();
        }

        public static void RefreshAccessToken()
        {
            _api.Settings.AccessToken = GetAccessToken();
        }

        private static void ConfigureLiveMonitor()
        {
            _monitor = new(_api);
            _monitor.SetChannelsByName(_monitorChannels);

            _monitor.OnServiceStarted += Monitor_OnServiceStarted;
            _monitor.OnStreamOnline += Monitor_OnStreamOnline;
            _monitor.OnStreamOffline += Monitor_OnStreamOffline;
            _monitor.OnStreamUpdate += Monitor_OnStreamUpdate;

            _monitor.Start();
        }

        private static void Monitor_OnServiceStarted(object sender, OnServiceStartedArgs e)
        {
            ConsoleOut("Monitor>CONNECTED", fontColor: ConsoleColor.Cyan);
        }

        private static void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {

        }

        private static void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
        {

        }

        private static void Monitor_OnStreamUpdate(object sender, OnStreamUpdateArgs e)
        {

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
