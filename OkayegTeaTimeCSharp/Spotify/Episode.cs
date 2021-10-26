using SpotifyAPI.Web;

namespace OkayegTeaTimeCSharp.Spotify;

public class Episode : PlayingItem
{
    public Episode(FullEpisode episode)
    {
        Artist = episode.Show.Publisher;
        Title = episode.Name;
        URI = episode.Uri;
        Message = $"{Title} by {Artist} || {URI}";
    }
}
