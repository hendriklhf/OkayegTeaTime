using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI = SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class SpotifyRequest
    {
        public async static Task GetNewAuthToken(string code)
        {
            var response = await new OAuthClient().RequestToken(new AuthorizationCodeTokenRequest(Resources.SpotifyClientID, Resources.SpotifyClientSecret, code, new Uri("example.com/callback")));
        }

        public static void GetCurrentlyPlaying(string accesToken)
        {
            Console.WriteLine(new SpotifyClient(accesToken).Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest()).Result.Context.Type);
        }
    }
}
