using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Chatterino7, typeof(Chatterino7Command))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct Chatterino7Command : IChatCommand<Chatterino7Command>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    private const string _responseMessage = "Website: 7tv.app || Releases: github.com/SevenTV/chatterino7/releases";

    public Chatterino7Command(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out Chatterino7Command command)
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
