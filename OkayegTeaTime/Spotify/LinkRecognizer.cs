using HLE.Strings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Spotify;

public class LinkRecognizer
{
    private static readonly Regex _spotifyLinkPattern = new(Pattern.SpotifyLink, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public string? FindSpotifyLink(ChatMessage chatMessage)
    {
        if (_spotifyLinkPattern.IsMatch(chatMessage.Message))
        {
            string uriCode = chatMessage.Message.Match(@"track/\w+\?").Remove("track/").Remove("?");
            return $"spotify:track:{uriCode}";
        }
        else
        {
            return null;
        }
    }
}
