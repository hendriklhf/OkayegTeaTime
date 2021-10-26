using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class SevenTvTarget
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}
