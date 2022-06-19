using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;

public class WeatherConditions
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}
