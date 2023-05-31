using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Api.SevenTv.Models.Responses;

internal readonly struct GetChannelEmotesResponse
{
    [JsonPropertyName("emote_set")]
    public required EmoteSet EmoteSet { get; init; } = EmoteSet.Empty;

    public GetChannelEmotesResponse()
    {
    }
}
