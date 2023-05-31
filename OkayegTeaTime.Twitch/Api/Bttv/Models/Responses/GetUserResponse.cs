using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Api.Bttv.Models.Responses;

internal readonly struct GetUserResponse
{
    [JsonPropertyName("channelEmotes")]
    public required Emote[] ChannelEmotes { get; init; } = Array.Empty<Emote>();

    [JsonPropertyName("sharedEmotes")]
    public required Emote[] SharedEmotes { get; init; } = Array.Empty<Emote>();

    public GetUserResponse()
    {
    }
}
