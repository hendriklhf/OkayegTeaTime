using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Formula1;

public sealed class Location
{
    [JsonPropertyName("lat")]
    public required string Latitude { get; set; }

    [JsonPropertyName("long")]
    public required string Longitude { get; set; }

    [JsonPropertyName("locality")]
    public required string Name { get; set; }

    [JsonPropertyName("country")]
    public required string Country { get; set; }
}
