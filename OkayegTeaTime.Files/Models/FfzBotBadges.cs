#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class FfzBotBadges
{
    [JsonPropertyName("2")]
    public string[] Users { get; set; }
}
