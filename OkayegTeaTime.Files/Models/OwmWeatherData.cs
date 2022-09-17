using System.Text.Json.Serialization;
using HLE.Time;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class OwmWeatherData
{
    [JsonPropertyName("coord")]
    public OwmCoordinates Coordinates { get; set; }

    [JsonPropertyName("weather")]
    public OwmWeatherCondition[] WeatherConditions { get; set; }

    [JsonPropertyName("base")]
    public string Base { get; set; }

    [JsonPropertyName("main")]
    public OwmWeather Weather { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public OwmWind Wind { get; set; }

    [JsonPropertyName("clouds")]
    public OwmCloud Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long DayTime { get; set; }

    [JsonPropertyName("sys")]
    public OwmLocation Location { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string CityName { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonIgnore]
    public long TimeOfRequest { get; } = TimeHelper.Now();
}
