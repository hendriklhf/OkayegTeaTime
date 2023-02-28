using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class Coordinates
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}
