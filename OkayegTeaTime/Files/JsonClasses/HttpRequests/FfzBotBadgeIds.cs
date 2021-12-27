using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzBotBadgeIds
{
    [JsonPropertyName("2")]
    public List<int> UserIds { get; set; }
}
