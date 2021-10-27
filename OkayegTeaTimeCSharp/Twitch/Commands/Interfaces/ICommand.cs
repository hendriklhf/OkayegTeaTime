using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.Interfaces;

public interface ICommand
{
    public TwitchBot TwitchBot { get; }

    public ITwitchChatMessage ChatMessage { get; }

    public string Alias { get; }

    public void Handle();
}
