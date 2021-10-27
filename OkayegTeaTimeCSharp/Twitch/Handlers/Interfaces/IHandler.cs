using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Handlers.Interfaces;

public interface IHandler
{
    public TwitchBot TwitchBot { get; }

    public void Handle(ITwitchChatMessage chatMessage);
}
