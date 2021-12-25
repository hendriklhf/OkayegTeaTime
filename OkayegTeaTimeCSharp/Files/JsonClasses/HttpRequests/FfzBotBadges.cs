using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class FfzBotBadges
{
    [JsonPropertyName("2")]
    public List<string> Users { get; set; }
}
