using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Controller;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;

public class FfzSets
{
    [JsonPropertyName(EmoteController.FfzSetIdReplacement)]
    public FfzMainSet EmoteSet { get; set; }
}
