#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzBotBadges
{
    [JsonPropertyName("2")]
    public List<string> Users { get; set; }
}
