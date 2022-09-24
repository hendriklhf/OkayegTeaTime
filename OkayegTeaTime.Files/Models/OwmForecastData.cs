using System;
using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class OwmForecastData
{
    [JsonPropertyName("city")]
    public OwmCity City { get; set; }

    [JsonPropertyName("cnt")]
    public byte Count { get; set; }

    [JsonPropertyName("list")]
    public OwmForecast[] Forecasts { get; set; }

    [JsonIgnore]
    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
