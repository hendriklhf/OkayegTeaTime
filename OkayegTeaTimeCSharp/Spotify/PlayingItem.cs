using SpotifyAPI.Web;

namespace OkayegTeaTimeCSharp.Spotify
{
    public class PlayingItem
    {
        public string Title { get; }

        public string Artist { get; }

        public string Message { get; private set; }

        public ItemType ItemType { get; }

        private readonly IPlayableItem _item;

        public PlayingItem(CurrentlyPlaying currentlyPlaying)
        {
        }
    }
}
