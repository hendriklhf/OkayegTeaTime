#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Formula1;

public sealed class Location
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
