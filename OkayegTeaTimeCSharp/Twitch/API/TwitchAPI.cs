using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public class TwitchAPI
    {
        public TwitchLib.Api.TwitchAPI API { get; private set; }

        private static TwitchAPI _twitchApi;

        public TwitchAPI()
        {
            API = new();
            API.Settings.ClientId = Resources.TwitchApiClientID;
            API.Settings.AccessToken = Resources.TwitchApiAccessToken;

            SetApi();
        }

        private void SetApi()
        {
            _twitchApi ??= this;
        }
    }
}
