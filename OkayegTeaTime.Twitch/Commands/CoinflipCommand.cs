using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using Random = HLE.Random;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Coinflip)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct CoinflipCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ref MessageBuilder _response;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public CoinflipCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        bool result = Random.StrongBool();
        string answer = result ? "yes/heads" : "no/tails";
        _response.Append(ChatMessage.Username, ", ", answer, " ", Emoji.Coin);
    }
}
