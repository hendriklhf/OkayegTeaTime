#nullable disable

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models.OpenWeatherMap;

public sealed class WeatherData
{
    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    [JsonPropertyName("weather")]
    public ImmutableArray<WeatherCondition> WeatherConditions { get; set; }

    [JsonPropertyName("base")]
    public string Base { get; set; }

    [JsonPropertyName("main")]
    public Weather Weather { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    [JsonPropertyName("clouds")]
    public Cloud Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long DayTime { get; set; }

    [JsonPropertyName("sys")]
    public Location Location { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string CityName { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonIgnore]
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
