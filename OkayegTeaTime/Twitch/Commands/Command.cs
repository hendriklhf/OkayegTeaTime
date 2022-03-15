using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public abstract class Command
{
    public TwitchBot TwitchBot { get; }

    public TwitchChatMessage ChatMessage { get; }

    public string? Prefix { get; }

    public string Alias { get; }

    public Response Response { get; protected set; } = new();

    public Command(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
    {
        TwitchBot = twitchBot;
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
            TwitchBot.Send(ChatMessage.Channel, message);
        }
    }
}
