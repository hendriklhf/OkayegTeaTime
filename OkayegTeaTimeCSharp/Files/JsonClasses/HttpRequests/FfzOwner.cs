using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.Files.JsonClasses.HttpRequests;

public class FfzOwner
{
    [JsonPropertyName("_id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
}
