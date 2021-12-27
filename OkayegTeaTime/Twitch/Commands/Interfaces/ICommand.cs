using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.Interfaces;

public interface ICommand
{
    public TwitchBot TwitchBot { get; }

    public ITwitchChatMessage ChatMessage { get; }

    public string Alias { get; }

    public void Handle();
}
