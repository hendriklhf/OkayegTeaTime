using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class SevenTvChange
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("values")]
    public List<string> Values { get; set; }
}
