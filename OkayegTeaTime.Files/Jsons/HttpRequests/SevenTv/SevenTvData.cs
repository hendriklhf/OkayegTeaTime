using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;

public class SevenTvData
{
    [JsonPropertyName("user")]
    public SevenTvUser User { get; set; }
}
