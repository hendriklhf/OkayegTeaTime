#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class FfzBotBadgeIds
{
    [JsonPropertyName("2")]
    public long[] UserIds { get; set; }
}
