using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class Formula1Location
{
    [JsonPropertyName("lat")]
    public string Latitude { get; set; }

    [JsonPropertyName("long")]
    public string Longitude { get; set; }

    [JsonPropertyName("locality")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }
}
