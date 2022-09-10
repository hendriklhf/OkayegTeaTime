using System.Collections.Generic;
using SpotifyAPI.Web;

#nullable disable

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

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

    public string Name { get; }

    public string Uri { get; }

    public string Url { get; }

    public bool IsLocal { get; }

    public bool IsTrack { get; }

    public bool IsEpisode { get; }

    public SpotifyItem(IPlayableItem item)
    {
        switch (item)
        {
            case FullTrack track:
            {
                Duration = track.DurationMs;
                Explicit = track.Explicit;
                ExternalUrls = track.ExternalUrls;
                Href = track.Href;
                Id = track.Id;
                IsPlayable = track.IsPlayable;
                Name = track.Name;
                Uri = track.Uri;
                Url = $"https://open.spotify.com/track/{Id}";
                IsLocal = track.IsLocal;
                IsTrack = true;
                break;
            }
            case FullEpisode episode:
            {
                Duration = episode.DurationMs;
                Explicit = episode.Explicit;
                ExternalUrls = episode.ExternalUrls;
                Href = episode.Href;
                Id = episode.Id;
                IsPlayable = episode.IsPlayable;
                Name = episode.Name;
                Uri = episode.Uri;
                Url = $"https://open.spotify.com/episode/{Id}";
                IsLocal = false;
                IsEpisode = true;
                break;
            }
        }
    }
}