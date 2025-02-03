using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models.Formula1;

public sealed class Race
{
    [JsonPropertyName("season")]
    public required string Season { get; set; }

    [JsonPropertyName("round")]
    public required string Round { get; set; }

    [JsonPropertyName("url")]
    public required string WikipediaUrl { get; set; }

    [JsonPropertyName("raceName")]
    public required string Name { get; set; }

    [JsonPropertyName("Circuit")]
    public required Circuit Circuit { get; set; }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [JsonIgnore]
    public Sessionn RaceSession { get; set; } = null!;

    [JsonIgnore]
    public Sessionn PracticeOneSession { get; set; } = null!;

    [JsonIgnore]
    public Sessionn PracticeTwoSession { get; set; } = null!;

    [JsonIgnore]
    public Sessionn? PracticeThreeSession { get; set; }

    [JsonIgnore]
    public Sessionn QualifyingSession { get; set; } = null!;
    // ReSharper restore PropertyCanBeMadeInitOnly.Global

    [JsonIgnore]
    public Sessionn? SprintSession { get; set; }

    [JsonIgnore]
    [MemberNotNullWhen(false, nameof(PracticeThreeSession))]
    [MemberNotNullWhen(true, nameof(SprintSession))]
    public bool HasSprintRace => SprintSession != default && PracticeThreeSession == default;
}
