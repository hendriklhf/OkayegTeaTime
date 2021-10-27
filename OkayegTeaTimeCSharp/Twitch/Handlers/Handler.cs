using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Handlers.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Handlers;

public abstract class Handler : IHandler
{
    public TwitchBot TwitchBot { get; }

    protected Handler(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
    }

    public abstract void Handle(ITwitchChatMessage chatMessage);
}
