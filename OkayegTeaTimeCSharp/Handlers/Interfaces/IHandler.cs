using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Handlers.Interfaces;

public interface IHandler
{
    public TwitchBot TwitchBot { get; }

    public void Handle(ITwitchChatMessage chatMessage);
}
