using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Json;

public sealed class TwitchSettings
{
    public required string Username { get; init; }

    public required string OAuthToken { get; init; }

    [JsonPropertyName("ApiClientId")]
    public required string ApiClientId { get; init; }

    public required string ApiClientSecret { get; init; }
}
