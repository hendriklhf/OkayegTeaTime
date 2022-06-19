using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;

public class Coordinates
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}
