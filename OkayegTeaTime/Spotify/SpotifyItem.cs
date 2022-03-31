using SpotifyAPI.Web;

#nullable disable

namespace OkayegTeaTime.Spotify;

public class SpotifyItem
{
    /// <summary>
    /// Duration in milliseconds.
    /// </summary>
    public int Duration { get; }

    public bool Explicit { get; }

    public Dictionary<string, string> ExternalUrls { get; }

    public string Href { get; }

    public string Id { get; }

    public bool IsPlayable { get; }

    public bool IsPlaying { get; }

    public string Name { get; }

    public string Uri { get; }

    public bool IsLocal { get; }

    public bool IsTrack { get; }

    public bool IsEpisode { get; }

    public SpotifyItem(CurrentlyPlaying item) : this(item.Item)
    {
        IsPlaying = item.IsPlaying;
    }

    public SpotifyItem(IPlayableItem item)
    {
        if (item is FullTrack track)
        {
            Duration = track.DurationMs;
            Explicit = track.Explicit;
            ExternalUrls = track.ExternalUrls;
            Href = track.Href;
            Id = track.Id;
            IsPlayable = track.IsPlayable;
            Name = track.Name;
            Uri = track.Uri;
            IsLocal = track.IsLocal;
            IsTrack = true;
        }
        else if (item is FullEpisode episode)
        {
            Duration = episode.DurationMs;
            Explicit = episode.Explicit;
            ExternalUrls = episode.ExternalUrls;
            Href = episode.Href;
            Id = episode.Id;
            IsPlayable = episode.IsPlayable;
            Name = episode.Name;
            Uri = episode.Uri;
            IsLocal = false;
            IsEpisode = true;
        }
    }
}
