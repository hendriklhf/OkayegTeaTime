using HLE.Strings;
using OkayegTeaTimeCSharp.Utils;
using SpotifyAPI.Web;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.Spotify
{
    public static class SpotifyHelper
    {
        public static string GetArtistString(this List<SimpleArtist> artist)
        {
            string result = string.Empty;
            artist.ForEach(artist =>
            {
                result += $"{artist.Name}, ";
            });
            return result.Trim()[..^1];
        }

        public static string GetSpotifyURI(string input)
        {
            if (input.IsMatch(Pattern.SpotifyUriPattern))
            {
                return input;
            }
            else if (input.IsMatch(Pattern.SpotifyLinkPattern))
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

        public static PlayingItem GetItem(this CurrentlyPlaying currentlyPlaying)
        {
            if (currentlyPlaying != null)
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
    }
}