using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class LinkRecognizer
    {
        public string Message { get; set; }

        public LinkRecognizer(IChatMessage chatMessage)
        {
            Message = chatMessage.Message;
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
