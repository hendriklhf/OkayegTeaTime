using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Handlers.Interfaces;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class Handler : IHandler
{
    public TwitchBot TwitchBot { get; }

    protected Handler(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
    }

    public abstract void Handle(ITwitchChatMessage chatMessage);
}
