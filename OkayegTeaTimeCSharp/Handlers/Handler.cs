using OkayegTeaTimeCSharp.Handlers.Interfaces;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Handlers
{
    public abstract class Handler : IHandler
    {
        public TwitchBot TwitchBot { get; }

        protected Handler(TwitchBot twitchBot)
        {
            TwitchBot = twitchBot;
        }

        public abstract void Handle(ITwitchChatMessage chatMessage);
    }
}
