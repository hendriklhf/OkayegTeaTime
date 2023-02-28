using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzUrls
{
    [JsonPropertyName("1")]
    public required string One { get; init; }

    [JsonPropertyName("2")]
    public required string Two { get; init; }

    [JsonPropertyName("4")]
    public required string Four { get; init; }
}
