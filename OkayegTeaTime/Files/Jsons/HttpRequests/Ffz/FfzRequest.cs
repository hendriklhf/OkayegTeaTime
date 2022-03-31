using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;

public class FfzRequest
{
    [JsonPropertyName("room")]
    public FfzRoom Room { get; set; }

    [JsonPropertyName("sets")]
    public FfzSets Set { get; set; }
}
