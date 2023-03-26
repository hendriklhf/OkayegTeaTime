using System.Text.Json.Serialization;

#nullable disable
#pragma warning disable CS0659

namespace OkayegTeaTime.Models.Ffz;

public sealed class Emote
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("owner")]
    public Owner Owner { get; set; }

    [JsonPropertyName("urls")]
    public Urls EmoteUrls { get; set; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is Emote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
