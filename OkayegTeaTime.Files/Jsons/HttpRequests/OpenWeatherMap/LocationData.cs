using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;

public class LocationData
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}
