using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class EmoteSet
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tags")]
    public object[] Tags { get; set; }

    [JsonPropertyName("immutable")]
    public bool Immutable { get; set; }

    [JsonPropertyName("privileged")]
    public bool Privileged { get; set; }

    [JsonPropertyName("emotes")]
    public Emote[] Emotes { get; set; }
}
