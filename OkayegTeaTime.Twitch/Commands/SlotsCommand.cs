﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Slots)]
public readonly unsafe ref struct SlotsCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private const byte _emoteSlotCount = 3;

    public SlotsCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex? emotePattern = null;
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            try
            {
                emotePattern = new(ChatMessage.Split[1], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.TheGivenPatternIsInvalid);
                return;
            }
        }

        IEnumerable<string> ffzEmotes = _twitchBot.EmoteController.GetFfzEmotes(ChatMessage.ChannelId).Concat(_twitchBot.EmoteController.FfzGlobalEmotes).Select(e => e.Name);
        IEnumerable<string> bttvEmotes = _twitchBot.EmoteController.GetBttvEmotes(ChatMessage.ChannelId).Concat(_twitchBot.EmoteController.BttvGlobalEmotes).Select(e => e.Name);
        IEnumerable<string> sevenTvEmotes = _twitchBot.EmoteController.GetSevenTvEmotes(ChatMessage.ChannelId).Select(e => e.Name).Concat(_twitchBot.EmoteController.SevenTvGlobalEmotes.Select(e => e.Name));
        string[] emotes = ffzEmotes.Concat(bttvEmotes).Concat(sevenTvEmotes).ToArray();

        if (emotes.Length == 0)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThereAreNoThirdPartyEmotesEnabledInThisChannel);
            return;
        }

        if (emotePattern is not null)
        {
            emotes = emotes.Where(e => emotePattern.IsMatch(e)).ToArray();
        }

        if (emotes.Length == 0)
        {
            Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.ThereIsNoEmoteMatchingYourProvidedPattern);
            return;
        }

        string[] randomEmotes = new string[_emoteSlotCount];
        for (int i = 0; i < _emoteSlotCount; i++)
        {
            randomEmotes[i] = emotes.Random()!;
        }

        Span<char> joinBuffer = stackalloc char[500];
        int bufferLength = StringHelper.Join(randomEmotes, ' ', joinBuffer);
        Span<char> lengthChars = stackalloc char[30];
        emotes.Length.TryFormat(lengthChars, out int lengthLength);
        lengthChars = lengthChars[..lengthLength];
        Response->Append(ChatMessage.Username, Messages.CommaSpace, "[ ", joinBuffer[..bufferLength], " ] (", lengthChars, " emote", emotes.Length > 1 ? "s" : string.Empty, ")");
    }
}
