using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models;

internal readonly struct EmoteSet
{
    [JsonPropertyName("emotes")]
    public required Emote[] Emotes { get; init; } = Array.Empty<Emote>();

    public static EmoteSet Empty => new()
    {
        Emotes = Array.Empty<Emote>()
    };

    public EmoteSet()
    {
    }
}
