using System.Diagnostics.CodeAnalysis;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Invalidate)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct InvalidateCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public InvalidateCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        if (!ChatMessage.IsBotModerator)
        {
            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.YouArentAModeratorOfTheBot);
            return;
        }

        _twitchBot.InvalidateCaches();
        Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, "all database cached have been invalidated");
    }
}
