using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch
{
    public abstract class Handler : IHandler
    {
        public TwitchBot TwitchBot { get; }

        protected Handler(TwitchBot twitchBot)
        {
            TwitchBot = twitchBot;
        }

        public abstract void Handle();
    }
}
