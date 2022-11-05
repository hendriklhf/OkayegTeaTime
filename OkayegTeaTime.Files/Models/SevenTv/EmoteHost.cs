using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models.SevenTv;

#nullable disable

public sealed class EmoteHost
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("files")]
    public EmoteFile[] Files { get; set; }
}
