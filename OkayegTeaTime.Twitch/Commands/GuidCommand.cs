using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Guid)]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct GuidCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref PoolBufferStringBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private const string _guidFormat = "D";

    public GuidCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
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
        _response.Append(ChatMessage.Username, ", ");
        guid.TryFormat(_response.FreeBufferSpan, out int guidLength, _guidFormat);
        _response.Advance(guidLength);
    }
}
