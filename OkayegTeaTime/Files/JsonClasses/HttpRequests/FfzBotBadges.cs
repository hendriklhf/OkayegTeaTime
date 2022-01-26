using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzBotBadges
{
    [JsonPropertyName("2")]
    public List<string> Users { get; set; }
}
