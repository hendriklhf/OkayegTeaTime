using System.Web;
using HLE.HttpRequests;
using OkayegTeaTimeCSharp.Exceptions;
using OkayegTeaTimeCSharp.Properties;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLibAPI = TwitchLib.Api.TwitchAPI;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public class TwitchAPI
    {
        private static readonly TwitchLibAPI _api = new();

        public void Configure()
        {
            _api.Settings.ClientId = Settings.TwitchApiClientID;
            _api.Settings.Secret = Settings.TwitchApiClientSecret;
            _api.Settings.AccessToken = GetAccessToken();
        }

        public string GetAccessToken()
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

        public void RefreshAccessToken()
        {
            _api.Settings.AccessToken = GetAccessToken();
        }

        public Channel GetChannelByName(string channel)
        {
            List<Channel> channels = _api.V5.Search.SearchChannelsAsync(HttpUtility.UrlEncode(channel), 20).Result.Channels.ToList();
            try
            {
                return channels.FirstOrDefault(c => c.Name == channel) ?? channels[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetChannelID(string channel)
        {
            string id = GetChannelByName(channel)?.Id;
            if (id is not null)
            {
                return id;
            }
            else
            {
                throw new UserNotFoundException();
            }
        }
    }
}
