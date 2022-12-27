using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Emojis;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Coinflip)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct CoinflipCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public CoinflipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        string result = Random.StrongBool() ? "yes/heads" : "no/tails";
        Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, result, StringHelper.Whitespace, Emoji.Coin);
    }
}
