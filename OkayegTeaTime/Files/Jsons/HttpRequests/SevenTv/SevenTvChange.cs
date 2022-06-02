using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;

public class SevenTvChange
{
    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("values")] public string[] Values { get; set; }
}
