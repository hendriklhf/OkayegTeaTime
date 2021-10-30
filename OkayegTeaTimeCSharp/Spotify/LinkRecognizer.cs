using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Spotify;

public class LinkRecognizer
{
    public string Message { get; set; }

    public LinkRecognizer(IChatMessage chatMessage)
    {
        Message = chatMessage.Message;
    }

    public bool TryFindSpotifyLink(out string uri)
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
