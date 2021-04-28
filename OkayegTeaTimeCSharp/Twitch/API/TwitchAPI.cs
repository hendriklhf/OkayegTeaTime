using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public class TwitchAPI
    {
        public static TwitchAPI TwitchApi;

        public TwitchLib.Api.TwitchAPI API { get; private set; }

        public TwitchAPI()
        {
            API = new();
            API.Settings.ClientId = Resources.TwitchApiClientID;
            API.Settings.AccessToken = Resources.TwitchApiAccessToken;

            SetApi();
        }

        private void SetApi()
        {
            TwitchApi = this;
        }
    }
}
