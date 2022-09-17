#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class SevenTvRequest
{
    [JsonPropertyName("data")]
    public SevenTvData Data { get; set; }
}
