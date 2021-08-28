using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using System.Collections.Generic;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class LinkRecognizer
    {
        public string Message { get; set; }

        public LinkRecognizer(ChatMessage chatMessage)
        {
            Message = chatMessage.GetMessage();
        }

        public bool FindSpotifyLink(out string uri)
        {
            if (Message.IsMatch(Pattern.SpotifyLink))
            {
                string uriCode = Message.Match(@"track/\w+\?").Remove("track/").Remove("?");
                uri = $"spotify:track:{uriCode}";
                return true;
            }
            else
            {
                uri = null;
                return false;
            }
        }
    }
}
