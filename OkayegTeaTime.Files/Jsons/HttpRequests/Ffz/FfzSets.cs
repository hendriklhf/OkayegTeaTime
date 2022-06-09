using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;

public class FfzSets
{
    [JsonPropertyName(AppSettings.FfzSetIdReplacement)]
    public FfzMainSet EmoteSet { get; set; }
}
