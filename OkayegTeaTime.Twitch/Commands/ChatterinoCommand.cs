using System.Diagnostics.CodeAnalysis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct ChatterinoCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly string? _prefix;
    private readonly string _alias;

    private const string _responseMessage = "Website: chatterino.com || Releases: github.com/Chatterino/chatterino2/releases";

    public ChatterinoCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        _response.Append(ChatMessage.Username, ", ", _responseMessage);
    }
}
