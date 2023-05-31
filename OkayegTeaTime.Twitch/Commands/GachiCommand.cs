using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Emojis;
using HLE.Twitch.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

#pragma warning disable IDE0052

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Gachi, typeof(GachiCommand))]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public readonly struct GachiCommand : IChatCommand<GachiCommand>
{
    public ResponseBuilder Response { get; }

    public ChatMessage ChatMessage { get; }

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlyMemory<char> _prefix;
    private readonly ReadOnlyMemory<char> _alias;

    public GachiCommand(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    {
        ChatMessage = chatMessage;
        Response = new(AppSettings.MaxMessageLength);
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public static void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out GachiCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public ValueTask Handle()
    {
        GachiSong? gachiSong = AppSettings.GachiSongs.Random();
        if (gachiSong is null)
        {
            Response.Append(Messages.CouldntFindASong);
            return ValueTask.CompletedTask;
        }

        Response.Append(Emoji.PointRight, " ", gachiSong.Title, " || ", gachiSong.Url, " gachiBASS");
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Response.Dispose();
    }
}
