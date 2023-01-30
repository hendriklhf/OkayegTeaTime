using System;
using System.Diagnostics.CodeAnalysis;
using HLE;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Guid)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct GuidCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public GuidCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Guid guid = Guid.NewGuid();
        Span<char> format = stackalloc char[1];
        format[0] = 'D';
        Span<char> chars = stackalloc char[50];
        guid.TryFormat(chars, out int guidLength, format);
        chars = chars[..guidLength];
        Response->Append(ChatMessage.Username, Messages.CommaSpace, chars);
    }
}
