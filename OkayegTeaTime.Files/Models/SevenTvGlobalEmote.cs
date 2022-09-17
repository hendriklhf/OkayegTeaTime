#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class SevenTvGlobalEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("provider")]
    public string Provider { get; set; }

    [JsonPropertyName("provider_id")]
    public string ProviderId { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("mime")]
    public string Mime { get; set; }

    [JsonPropertyName("owner")]
    public SevenTvGlobalEmoteOwner Owner { get; set; }
}
