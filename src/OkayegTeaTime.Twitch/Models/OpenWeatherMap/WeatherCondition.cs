#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models.OpenWeatherMap;

public sealed class WeatherCondition
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}
