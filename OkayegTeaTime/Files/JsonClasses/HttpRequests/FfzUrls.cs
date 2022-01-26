using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzUrls
{
    [JsonPropertyName("1")]
    public string One { get; set; }

    [JsonPropertyName("2")]
    public string Two { get; set; }

    [JsonPropertyName("4")]
    public string Four { get; set; }
}
