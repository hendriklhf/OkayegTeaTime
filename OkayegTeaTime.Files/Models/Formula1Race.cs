using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Models;

public class Formula1Race
{
    [JsonPropertyName("season")]
    public string Season { get; set; }

    [JsonPropertyName("round")]
    public string Round { get; set; }

    [JsonPropertyName("url")]
    public string WikipediaUrl { get; set; }

    [JsonPropertyName("raceName")]
    public string Racename { get; set; }

    [JsonPropertyName("Circuit")]
    public Formula1Circuit Circuit { get; set; }

    [JsonIgnore]
    public Formula1Session Race { get; set; }

    [JsonIgnore]
    public Formula1Session PracticeOne { get; set; }

    [JsonIgnore]
    public Formula1Session PracticeTwo { get; set; }

    [JsonIgnore]
    public Formula1Session PracticeThree { get; set; }

    [JsonIgnore]
    public Formula1Session Qualifying { get; set; }

    [JsonIgnore]
    public Formula1Session Sprint { get; set; }

    [JsonIgnore]
    public bool HasSprintRace => Sprint != default && PracticeThree == default;
}
