using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class Wind
{
    [JsonPropertyName("speed")]
    public required double Speed { get; init; }

    [JsonPropertyName("deg")]
    public required double Direction { get; init; }

    [JsonPropertyName("gust")]
    public required double Gust { get; init; }
}
