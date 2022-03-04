using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;

public class SevenTvChange
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("values")]
    public List<string> Values { get; set; }
}
