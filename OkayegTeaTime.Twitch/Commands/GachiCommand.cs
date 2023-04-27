using System;
using System.Diagnostics.CodeAnalysis;
using HLE.Collections;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Gachi)]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly ref struct GachiCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref PoolBufferStringBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    public GachiCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref PoolBufferStringBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        GachiSong? gachiSong = AppSettings.GachiSongs.Random();
        if (gachiSong is null)
        {
            _response.Append(Messages.CouldntFindASong);
            return;
        }

        _response.Append(Emoji.PointRight, " ", gachiSong.Title, " || ", gachiSong.Url, " gachiBASS");
    }
}
