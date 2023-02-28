#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzRequest
{
    [JsonPropertyName("room")]
    public FfzRoom Room { get; set; }

    [JsonPropertyName("sets")]
    public FfzSets Set { get; set; }
}
