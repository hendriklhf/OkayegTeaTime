using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Api.Ffz.Models.Responses;

internal readonly struct GetRoomResponse
{
    [JsonPropertyName("room")]
    public required Room Room { get; init; } = Room.Empty;

    public GetRoomResponse()
    {
    }
}
