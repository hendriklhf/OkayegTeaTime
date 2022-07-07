using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public double Direction { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}
