using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models.Responses;

internal readonly struct GetGlobalEmotesResponse
{
    [JsonPropertyName("sets")]
    public required Sets Sets { get; init; }
}

internal readonly struct Sets
{
    [JsonPropertyName("3")]
    public required GlobalSet GlobalSet { get; init; }
}

internal readonly struct GlobalSet
{
    [JsonPropertyName("emoticons")]
    public required Emote[] Emotes { get; init; } = Array.Empty<Emote>();

    public GlobalSet()
    {
    }
}
