using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands;

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
