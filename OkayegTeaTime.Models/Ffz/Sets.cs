#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class Sets
{
    [JsonPropertyName("mainSet")]
    public MainSet EmoteSet { get; set; }
}
