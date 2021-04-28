namespace OkayegTeaTimeCSharp.Twitch.API
{
    public class TwitchAPI
    {
        public static TwitchAPI TwitchApi;

        public TwitchLib.Api.TwitchAPI API { get; private set; }

        public TwitchAPI()
        {
            Config.GetClientID();
            Config.GetAccessToken();

            API = new();
            API.Settings.ClientId = Config.ClientID;
            API.Settings.AccessToken = Config.AccessToken;

            SetApi();
        }

        private void SetApi()
        {
            TwitchApi = this;
        }
    }
}
