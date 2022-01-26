using System.Text.Json.Serialization;
using OkayegTeaTime.HttpRequests;

#nullable disable

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzSets
{
    [JsonPropertyName(HttpRequest.FfzSetIdReplacement)]
    public FfzMainSet EmoteSet { get; set; }
}
