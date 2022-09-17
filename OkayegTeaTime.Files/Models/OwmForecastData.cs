using System.Text.Json.Serialization;
using HLE.Time;

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
    public long TimeOfRequest { get; } = TimeHelper.Now();
}
