using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzUrls
{
    [JsonPropertyName("1")]
    public string One { get; init; }

    [JsonPropertyName("2")]
    public string Two { get; init; }

    [JsonPropertyName("4")]
    public string Four { get; init; }
}
