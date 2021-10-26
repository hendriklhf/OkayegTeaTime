using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class FfzUrls
{
    [JsonPropertyName("1")]
    public string One { get; set; }

    [JsonPropertyName("2")]
    public string Two { get; set; }

    [JsonPropertyName("4")]
    public string Four { get; set; }
}
