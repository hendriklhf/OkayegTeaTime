using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests.Ffz;

public class FfzBotBadgeIds
{
    [JsonPropertyName("2")]
    public List<int> UserIds { get; set; }
}
