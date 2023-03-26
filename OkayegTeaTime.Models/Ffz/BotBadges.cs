#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class BotBadges
{
    [JsonPropertyName("2")]
    public string[] Users { get; set; }
}
