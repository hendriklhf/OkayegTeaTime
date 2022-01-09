using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public abstract class Command
{
    public TwitchBot TwitchBot { get; }

    public TwitchChatMessage ChatMessage { get; }

    public string Alias { get; }

    public Response Response { get; set; } = new();

    public Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        TwitchBot = twitchBot;
        ChatMessage = chatMessage;
        Alias = alias;
    }

    public virtual void Handle()
    {
        TwitchBot.Send(ChatMessage.Channel, Response);
    }
}
