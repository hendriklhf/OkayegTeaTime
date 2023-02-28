using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.SevenTv;

#nullable disable

public sealed class EmoteOwner
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonPropertyName("style")]
    public Style Style { get; set; }

    [JsonPropertyName("roles")]
    public string[] Roles { get; set; }

    [JsonPropertyName("connections")]
    public object Connections { get; set; }
}
