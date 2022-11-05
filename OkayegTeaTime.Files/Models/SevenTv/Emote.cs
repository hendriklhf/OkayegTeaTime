using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class Emote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("actor_id")]
    public object ActorId { get; set; }

    [JsonPropertyName("data")]
    public EmoteData EmoteData { get; set; }
}
