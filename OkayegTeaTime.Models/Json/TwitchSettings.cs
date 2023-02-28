#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Json;

public sealed class TwitchSettings
{
    public string Username { get; set; }

    public string OAuthToken { get; set; }

    [JsonPropertyName("ApiClientId")]
    public string ApiClientId { get; set; }

    public string ApiClientSecret { get; set; }
}
