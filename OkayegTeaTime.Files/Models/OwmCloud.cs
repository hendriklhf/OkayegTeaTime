#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class OwmCloud
{
    [JsonPropertyName("all")]
    public int Percentage { get; set; }
}
