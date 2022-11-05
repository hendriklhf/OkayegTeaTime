using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class EmoteFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("static_name")]
    public string StaticName { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }
}
