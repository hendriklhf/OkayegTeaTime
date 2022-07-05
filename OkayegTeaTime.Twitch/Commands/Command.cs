using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public abstract class Command
{
    protected TwitchChatMessage ChatMessage { get; }

    protected string? Prefix { get; }

    protected string Alias { get; }

    public Response Response { get; protected set; } = new();

    private protected readonly TwitchBot _twitchBot;

    protected Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        _twitchBot = twitchBot;
        ChatMessage = chatMessage;
        Prefix = DbControl.Channels[chatMessage.ChannelId]?.Prefix;
        Alias = alias;
    }

    public abstract void Handle();

    public void SendResponse()
    {
        string message = Response.Message;
        if (!message.IsNullOrEmptyOrWhitespace())
        {
            _twitchBot.Send(ChatMessage.Channel, message);
        }
    }
}
