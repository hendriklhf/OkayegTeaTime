using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;

public class SevenTvRequest
{
    [JsonPropertyName("data")]
    public SevenTvData Data { get; set; }
}
