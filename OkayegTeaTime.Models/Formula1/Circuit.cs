#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Formula1;

public sealed class Circuit
{
    [JsonPropertyName("circuitId")]
    public string Id { get; set; }

    [JsonPropertyName("url")]
    public string WikipediaUrl { get; set; }

    [JsonPropertyName("circuitName")]
    public string Name { get; set; }

    [JsonPropertyName("Location")]
    public Location Location { get; set; }
}
