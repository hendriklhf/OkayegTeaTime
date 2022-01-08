using HLE.Strings;
using SpotifyAPI.Web;

namespace OkayegTeaTime.Spotify;

public static class SpotifyHelper
{
    public static string GetArtists(this List<SimpleArtist> artist)
    {
        return string.Join(", ", artist.Select(a => a.Name));
    }

    public static FullTrack? GetExcactTrackFromSearch(List<FullTrack>? tracks, List<string> query)
    {
        return tracks?.FirstOrDefault(t =>
            query.Any(q => t.Name.IsMatch(q))
            || query.Any(q => t.Artists.Any(a => a.Name.IsMatch(q))));
    }

    public static PlayingItem? GetItem(this CurrentlyPlaying currentlyPlaying)
    {
        if (currentlyPlaying is not null)
        {
            if (currentlyPlaying.Item is FullTrack track)
            {
                return new Track(track);
            }
            else if (currentlyPlaying.Item is FullEpisode episode)
            {
                return new Episode(episode);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
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
