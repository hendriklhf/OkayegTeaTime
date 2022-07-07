#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class BttvUser
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("providerId")]
    public string UserId { get; set; }
}
