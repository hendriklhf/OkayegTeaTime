using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.SevenTv.Models.Responses;

internal readonly struct GetGlobalEmotesResponse
{
    [JsonPropertyName("emotes")]
    public required Emote[] Emotes { get; init; }
}
