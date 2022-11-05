using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class EmoteData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("lifecycle")]
    public int LifeCycle { get; set; }

    [JsonPropertyName("listed")]
    public bool Listed { get; set; }

    [JsonPropertyName("animated")]
    public bool Animated { get; set; }

    [JsonPropertyName("owner")]
    public EmoteOwner Owner { get; set; }

    [JsonPropertyName("host")]
    public EmoteHost Host { get; set; }
}
