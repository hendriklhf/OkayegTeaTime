#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class OwmCloud
{
    [JsonPropertyName("all")]
    public int Percentage { get; set; }
}
