using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Twitch.Interfaces
{
    public interface IHandler
    {
        public TwitchBot TwitchBot { get; }

        public void Handle();
    }
}
