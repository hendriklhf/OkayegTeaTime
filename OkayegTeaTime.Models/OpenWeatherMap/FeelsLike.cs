using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class FeelsLike
{
    [JsonPropertyName("day")]
    public double Day { get; set; }

    [JsonPropertyName("night")]
    public double Night { get; set; }

    [JsonPropertyName("eve")]
    public double Evening { get; set; }

    [JsonPropertyName("morn")]
    public double Morning { get; set; }
}
