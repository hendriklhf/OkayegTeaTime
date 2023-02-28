#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.OpenWeatherMap;

public sealed class City
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("population")]
    public ulong Population { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }
}
