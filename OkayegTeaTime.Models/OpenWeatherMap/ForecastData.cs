#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class ForecastData
{
    [JsonPropertyName("city")]
    public City City { get; set; }

    [JsonPropertyName("cnt")]
    public byte Count { get; set; }

    [JsonPropertyName("list")]
    public Forecast[] Forecasts { get; set; }

    [JsonIgnore]
    public DateTime TimeOfRequest { get; } = DateTime.UtcNow;
}
