using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public class Episode : PlayingItem
{
    public Episode(FullEpisode episode)
    {
        Artists = new[] { episode.Show.Publisher };
        Title = episode.Name;
        Uri = episode.Uri;
        Message = $"{Title} by {string.Join(", ", Artists)} || {Uri}";
        IsLocal = false;
    }
}
