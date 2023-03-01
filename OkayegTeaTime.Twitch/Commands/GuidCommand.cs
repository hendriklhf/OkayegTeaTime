using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Guid)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct GuidCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly string? _prefix;
    private readonly string _alias;

    private const string _guidFormat = "D";

    public GuidCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Guid guid = Guid.NewGuid();
        Span<char> guidChars = stackalloc char[50];
        guid.TryFormat(guidChars, out int guidLength, _guidFormat);
        _response.Append(ChatMessage.Username, ", ", guidChars[..guidLength]);
    }
}
