using System.Collections.Generic;
using SpotifyAPI.Web;

#nullable disable

namespace OkayegTeaTime.Spotify;

public class SpotifyTrack : SpotifyItem
{
    public SimpleAlbum Album { get; }

    public List<SimpleArtist> Artists { get; }

    public List<string> AvailableMarkets { get; }

    public int DiscNumber { get; }

    public Dictionary<string, string> ExternalIds { get; }

    public LinkedTrack LinkedFrom { get; }

    public Dictionary<string, string> Restrictions { get; }

    public int Popularity { get; }

    public string PreviewUrl { get; }

    public int TrackNumber { get; }

    public SpotifyTrack(CurrentlyPlaying item) : this(item.Item)
    {
    }

    public SpotifyTrack(IPlayableItem item) : base(item)
    {
        if (item is not FullTrack track)
        {
            return;
        }

        Album = track.Album;
        Artists = track.Artists;
        AvailableMarkets = track.AvailableMarkets;
        DiscNumber = track.DiscNumber;
        ExternalIds = track.ExternalIds;
        LinkedFrom = track.LinkedFrom;
        Restrictions = track.Restrictions;
        Popularity = track.Popularity;
        PreviewUrl = track.PreviewUrl;
        TrackNumber = track.TrackNumber;
    }
}
