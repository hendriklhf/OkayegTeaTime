using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public abstract class Command
{
    protected TwitchChatMessage ChatMessage { get; }

    protected Response Response { get; private protected set; } = Response.Empty;

    private protected readonly TwitchBot _twitchBot;
    private protected readonly string? _prefix;
    private protected readonly string _alias;

    protected Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        _twitchBot = twitchBot;
        ChatMessage = chatMessage;
        _prefix = twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        _alias = alias;
    }

    public abstract void Handle();

    public void SendResponse()
    {
        string message = Response;
        if (!message.IsNullOrEmptyOrWhitespace())
        {
            _twitchBot.Send(ChatMessage.Channel, message);
        }
    }
}
