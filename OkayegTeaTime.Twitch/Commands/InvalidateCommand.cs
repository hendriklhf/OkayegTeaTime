using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Invalidate)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly ref struct InvalidateCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public InvalidateCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        using ChatMessageExtension messageExtension = new(ChatMessage);
        if (!messageExtension.IsBotModerator)
        {
            _response.Append(ChatMessage.Username, ", ", Messages.YouArentAModeratorOfTheBot);
            return;
        }

        _twitchBot.InvalidateCaches();
        _response.Append(ChatMessage.Username, ", ", "all database caches have been invalidated");
    }
}
