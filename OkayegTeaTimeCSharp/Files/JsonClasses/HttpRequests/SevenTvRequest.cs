using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class SevenTvRequest
{
    [JsonPropertyName("data")]
    public SevenTvData Data { get; set; }
}
