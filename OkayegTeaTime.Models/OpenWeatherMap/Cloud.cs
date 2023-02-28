#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class Cloud
{
    [JsonPropertyName("all")]
    public int Percentage { get; set; }
}
