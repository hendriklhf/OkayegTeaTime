using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; init; }

    [JsonPropertyName("deg")]
    public double Direction { get; init; }

    [JsonPropertyName("gust")]
    public double Gust { get; init; }
}
