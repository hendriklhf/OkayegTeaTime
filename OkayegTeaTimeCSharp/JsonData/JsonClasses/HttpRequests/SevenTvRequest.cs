using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class SevenTvRequest
{
    [JsonPropertyName("data")]
    public SevenTvData Data { get; set; }
}
