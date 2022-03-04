using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests.SevenTv;

public class SevenTvData
{
    [JsonPropertyName("user")]
    public SevenTvUser User { get; set; }
}
