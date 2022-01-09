using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers.Interfaces;

public interface IHandler
{
    public TwitchBot TwitchBot { get; }

    public void Handle(TwitchChatMessage chatMessage);
}
