#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public class FfzSets
{
    [JsonPropertyName(AppSettings.FfzSetIdReplacement)]
    public FfzMainSet EmoteSet { get; set; }
}
