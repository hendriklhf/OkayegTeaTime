using SpotifyAPI.Web;

#nullable disable

namespace OkayegTeaTime.Spotify;

public class SpotifyEpisode : SpotifyItem
{
    public string AudioPreviewUrl { get; }

    public string Description { get; }

    public List<Image> Images { get; }

    public bool IsExternallyHosted { get; }

    public List<string> Languages { get; }

    public string ReleaseDate { get; }

    public string ReleaseDatePrecision { get; }

    public ResumePoint ResumePoint { get; }

    public SimpleShow Show { get; }

    public SpotifyEpisode(CurrentlyPlaying item) : this(item.Item)
    {
    }

    public SpotifyEpisode(IPlayableItem item) : base(item)
    {
        if (item is FullEpisode episode)
        {
            AudioPreviewUrl = episode.AudioPreviewUrl;
            Description = episode.Description;
            Images = episode.Images;
            IsExternallyHosted = episode.IsExternallyHosted;
            Languages = episode.Languages;
            ReleaseDate = episode.ReleaseDate;
            ReleaseDatePrecision = episode.ReleaseDatePrecision;
            ResumePoint = episode.ResumePoint;
            Show = episode.Show;
        }
    }
}
