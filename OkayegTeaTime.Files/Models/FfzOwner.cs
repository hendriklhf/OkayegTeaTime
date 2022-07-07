#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class FfzOwner
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
}
