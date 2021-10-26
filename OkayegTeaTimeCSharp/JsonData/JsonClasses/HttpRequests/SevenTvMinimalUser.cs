using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class SevenTvMinimalUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("login")]
    public string Username { get; set; }

    [JsonPropertyName("role")]
    public SevenTvRole Role { get; set; }

    [JsonPropertyName("profile_image_url")]
    public string ProfilePictureUrl { get; set; }

    [JsonPropertyName("emote_ids")]
    public List<string> EmoteIds { get; set; }
}
