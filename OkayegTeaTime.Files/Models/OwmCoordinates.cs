using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class OwmCoordinates
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}
