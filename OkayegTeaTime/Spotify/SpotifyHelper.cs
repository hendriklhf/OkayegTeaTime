using HLE.Strings;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyHelper
{
    public static string[] GetArtistNames(this List<SimpleArtist> artists)
    {
        return artists.Select(a => a.Name).ToArray();
    }

    public static FullTrack? GetExcactTrackFromSearch(List<FullTrack>? tracks, List<string> query)
    {
        return tracks?.FirstOrDefault(t =>
            query.Any(q => t.Name.IsMatch(q))
            || query.Any(q => t.Artists.Any(a => a.Name.IsMatch(q))));
    }

    public static PlayingItem? GetPlayingItem(CurrentlyPlaying currentlyPlaying)
    {
        if (currentlyPlaying is null)
        {
            return null;
        }

        if (currentlyPlaying.Item is FullTrack track)
        {
            return new Track(track);
        }
        else if (currentlyPlaying.Item is FullEpisode episode)
        {
            return new Episode(episode);
        }

        return null;
    }

    public static string? GetSpotifyUri(string input)
    {
        if (input.IsMatch(Pattern.SpotifyUri))
        {
            return input;
        }
        else if (input.IsMatch(Pattern.SpotifyLink))
        {
            string uriCode = input.Match(@"track/\w+\?").Remove("track/").Remove("?");
            return $"spotify:track:{uriCode}";
        }
        else if (input.IsMatch(@"\w{22}"))
        {
            return $"spotify:track:{input}";
        }
        else
        {
            return null;
        }
    }
}
