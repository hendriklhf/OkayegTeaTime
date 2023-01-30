﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly string[] _answers =
    {
        "no/tails",
        "yes/heads"
    };

    public CoinflipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
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
        string answer = _answers[Unsafe.As<bool, byte>(ref result)];
        Response->Append(ChatMessage.Username, Messages.CommaSpace, answer, StringHelper.Whitespace, Emoji.Coin);
    }
}
