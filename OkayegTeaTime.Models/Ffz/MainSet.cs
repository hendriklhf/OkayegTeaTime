#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class MainSet
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("emoticons")]
    public Emote[] Emotes { get; set; }
}
