using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Handlers.Interfaces;

public interface IHandler
{
    public TwitchBot TwitchBot { get; }

    public void Handle(ITwitchChatMessage chatMessage);
}
