#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class SevenTvData
{
    [JsonPropertyName("user")]
    public SevenTvUser User { get; set; }
}
