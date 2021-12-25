using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class SevenTvData
{
    [JsonPropertyName("user")]
    public SevenTvUser User { get; set; }
}
