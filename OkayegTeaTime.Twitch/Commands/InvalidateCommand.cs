using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Invalidate)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct InvalidateCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public InvalidateCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!messageExtension.IsBotModerator)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.YouArentAModeratorOfTheBot);
            return;
        }

        _twitchBot.InvalidateCaches();
        Response->Append(ChatMessage.Username, Messages.CommaSpace, "all database caches have been invalidated");
    }
}
