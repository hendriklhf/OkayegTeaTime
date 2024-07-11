using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models.Formula1;

public sealed class Circuit
{
    [JsonPropertyName("circuitId")]
    public required string Id { get; set; }

    [JsonPropertyName("url")]
    public required string WikipediaUrl { get; set; }

    [JsonPropertyName("circuitName")]
    public required string Name { get; set; }

    [JsonPropertyName("Location")]
    public required Location Location { get; set; }
}
