#nullable disable

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
