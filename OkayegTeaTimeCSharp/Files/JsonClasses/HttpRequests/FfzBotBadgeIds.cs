using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class FfzBotBadgeIds
{
    [JsonPropertyName("2")]
    public List<int> UserIds { get; set; }
}
