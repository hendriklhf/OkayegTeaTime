using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.Settings;

public class Twitch
{
    public string Username { get; set; }

    public string OAuthToken { get; set; }

    [JsonPropertyName("ApiClientId")]
    public string ApiClientId { get; set; }

    public string ApiClientSecret { get; set; }
}
