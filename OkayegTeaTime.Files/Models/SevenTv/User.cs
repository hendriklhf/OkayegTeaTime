using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("platform")]
    public string Platform { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    [JsonPropertyName("linked_at")]
    public long LinkedAt { get; set; }

    [JsonPropertyName("emote_capacity")]
    public int EmoteCapacity { get; set; }

    [JsonPropertyName("emote_set")]
    public EmoteSet EmoteSet { get; set; }
}
