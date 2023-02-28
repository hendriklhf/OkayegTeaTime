#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzSets
{
    [JsonPropertyName("mainSet")]
    public FfzMainSet EmoteSet { get; set; }
}
