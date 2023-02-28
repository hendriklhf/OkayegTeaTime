#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Formula1;

public sealed class Race
{
    [JsonPropertyName("season")]
    public string Season { get; set; }

    [JsonPropertyName("round")]
    public string Round { get; set; }

    [JsonPropertyName("url")]
    public string WikipediaUrl { get; set; }

    [JsonPropertyName("raceName")]
    public string Name { get; set; }

    [JsonPropertyName("Circuit")]
    public Circuit Circuit { get; set; }

    [JsonIgnore]
    public Session RaceSession { get; set; }

    [JsonIgnore]
    public Session PracticeOneSession { get; set; }

    [JsonIgnore]
    public Session PracticeTwoSession { get; set; }

    [JsonIgnore]
    public Session PracticeThreeSession { get; set; }

    [JsonIgnore]
    public Session QualifyingSession { get; set; }

    [JsonIgnore]
    public Session SprintSession { get; set; }

    [JsonIgnore]
    public bool HasSprintRace => SprintSession != default && PracticeThreeSession == default;
}
