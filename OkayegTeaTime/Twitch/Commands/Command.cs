using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Interfaces;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands;

public abstract class Command : ICommand
{
    public TwitchBot TwitchBot { get; }

    public ITwitchChatMessage ChatMessage { get; }

    public string Alias { get; }

    public Command(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
    {
        TwitchBot = twitchBot;
        ChatMessage = chatMessage;
        Alias = alias;
    }

    public abstract void Handle();
}
