using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class SevenTvData
{
    [JsonPropertyName("user")]
    public SevenTvUser User { get; set; }
}
