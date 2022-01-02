using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public class Episode : PlayingItem
{
    public Episode(FullEpisode episode)
    {
        Artist = episode.Show.Publisher;
        Title = episode.Name;
        Uri = episode.Uri;
        Message = $"{Title} by {Artist} || {Uri}";
    }
}
