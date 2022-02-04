using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public class Track : PlayingItem
{
    public Track(FullTrack track)
    {
        Artists = track.Artists.GetArtistNames();
        Title = track.Name;
        Uri = track.IsLocal ? "local file" : track.Uri;
        Message = $"{Title} by {string.Join(", ", Artists)} || {Uri}";
        IsLocal = track.IsLocal;
    }
}
