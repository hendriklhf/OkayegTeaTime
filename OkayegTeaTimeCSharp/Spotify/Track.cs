using HLE.Strings;
using SpotifyAPI.Web;

namespace OkayegTeaTimeCSharp.Spotify;

public class Track : PlayingItem
{
    public Track(FullTrack track)
    {
        Artist = track.Artists.GetArtists();
        Title = track.Name;
        URI = track.Uri.IsMatch("local") ? "local file" : track.Uri;
        Message = $"{Title} by {Artist} || {URI}";
    }
}
