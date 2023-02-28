using System;
using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Guid)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct GuidCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private const string _guidFormat = "D";

    public GuidCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
        Span<char> guidChars = stackalloc char[50];
        guid.TryFormat(guidChars, out int guidLength, _guidFormat);
        Response->Append(ChatMessage.Username, Messages.CommaSpace, guidChars[..guidLength]);
    }
}
