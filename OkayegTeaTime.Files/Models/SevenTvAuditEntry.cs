#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class SevenTvAuditEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    [JsonPropertyName("action_user_id")]
    public string ActionUserId { get; set; }

    [JsonPropertyName("action_user")]
    public SevenTvMinimalUser ActionUser { get; set; }

    [JsonPropertyName("changes")]
    public SevenTvChange[] Changes { get; set; }

    [JsonPropertyName("target")]
    public SevenTvTarget Target { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; }
}
