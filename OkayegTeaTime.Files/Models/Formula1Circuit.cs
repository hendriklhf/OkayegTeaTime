using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class Formula1Circuit
{
    [JsonPropertyName("circuitId")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string WikipediaUrl { get; set; }

    [JsonPropertyName("circuitName")]
    public string Name { get; set; }

    [JsonPropertyName("Location")]
    public Formula1Location Location { get; set; }
}
