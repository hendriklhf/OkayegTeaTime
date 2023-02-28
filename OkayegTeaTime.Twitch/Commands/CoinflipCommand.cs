using System.Diagnostics.CodeAnalysis;
using HLE;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Coinflip)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
public readonly unsafe ref struct CoinflipCommand
{
    public ChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    public CoinflipCommand(TwitchBot twitchBot, ChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        bool result = Random.StrongBool();
        string answer = result ? "yes/heads" : "no/tails";
        Response->Append(ChatMessage.Username, Messages.CommaSpace, answer, StringHelper.Whitespace, Emoji.Coin);
    }
}
