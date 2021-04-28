using System.Collections.Generic;
using System.Threading.Tasks;

using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users;
using TwitchLib.Api.V5.Models.Subscriptions;

namespace OkayegTeaTimeCSharp.Twitch.API
{
    public class TwitchAPI
    {
        public static TwitchAPI TwitchApi;

        public TwitchLib.Api.TwitchAPI twitchAPI { get; private set; }

        public TwitchAPI()
        {
            twitchAPI = new();
            twitchAPI.Settings.ClientId = "";
            twitchAPI.Settings.AccessToken = "";

            SetApi();
        }

        private void SetApi()
        {
            TwitchApi = this;
        }
    }
}
