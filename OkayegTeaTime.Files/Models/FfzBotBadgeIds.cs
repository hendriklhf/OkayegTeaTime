#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class FfzBotBadgeIds
{
    [JsonPropertyName("2")]
    public long[] UserIds { get; set; }
}
