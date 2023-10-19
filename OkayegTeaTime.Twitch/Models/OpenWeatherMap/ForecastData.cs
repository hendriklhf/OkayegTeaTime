#nullable disable

using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models.OpenWeatherMap;

public sealed class ForecastData
{
    [JsonPropertyName("city")]
    public City City { get; set; }

    [JsonPropertyName("cnt")]
    public byte Count { get; set; }

    [JsonPropertyName("list")]
    public ImmutableArray<Forecast> Forecasts { get; set; }

    [JsonIgnore]
    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
