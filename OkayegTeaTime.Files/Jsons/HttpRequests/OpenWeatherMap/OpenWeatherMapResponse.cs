using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;

public class OpenWeatherMapResponse
{
    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    [JsonPropertyName("weather")]
    public WeatherConditions[] WeatherConditions { get; set; }

    [JsonPropertyName("base")]
    public string Base { get; set; }

    [JsonPropertyName("main")]
    public WeatherData Weather { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; }

    [JsonPropertyName("clouds")]
    public CloudData Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long DayTime { get; set; }

    [JsonPropertyName("sys")]
    public LocationData Location { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string CityName { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
