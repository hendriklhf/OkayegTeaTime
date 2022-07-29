using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public class OwmForecast
{
    [JsonPropertyName("dt")]
    public long DayTime { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }

    [JsonPropertyName("temp")]
    public OwmTemperature Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public OwmFeelsLike FeelsLike { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("weather")]
    public OwmWeatherCondition[] WeatherConditions { get; set; }

    [JsonPropertyName("speed")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("deg")]
    public int Direction { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }

    [JsonPropertyName("clouds")]
    public int CloudCover { get; set; }

    [JsonPropertyName("pop")]
    public double Pop { get; set; }
}
