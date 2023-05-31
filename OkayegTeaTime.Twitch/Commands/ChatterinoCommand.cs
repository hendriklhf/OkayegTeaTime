using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino, typeof(ChatterinoCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct ChatterinoCommand : IChatCommand<ChatterinoCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private const string _responseMessage = "Website: chatterino.com || Releases: github.com/Chatterino/chatterino2/releases";

    public ChatterinoCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out ChatterinoCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        Response.Append(ChatMessage.Username, ", ", _responseMessage);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
