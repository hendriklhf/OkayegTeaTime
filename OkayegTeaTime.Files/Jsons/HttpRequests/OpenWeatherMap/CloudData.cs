using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.OpenWeatherMap;

public class CloudData
{
    [JsonPropertyName("all")]
    public int Percentage { get; set; }
}
