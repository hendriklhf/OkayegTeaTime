using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class Handler
{
    private protected readonly TwitchBot _twitchBot;

    protected Handler(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

    public abstract void Handle(TwitchChatMessage chatMessage);
}
