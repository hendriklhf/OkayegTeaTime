using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;

public class SevenTvRole
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("color")]
    public int Color { get; set; }

    [JsonPropertyName("allowed")]
    public string Allowed { get; set; }

    [JsonPropertyName("denied")]
    public string Denied { get; set; }
}
