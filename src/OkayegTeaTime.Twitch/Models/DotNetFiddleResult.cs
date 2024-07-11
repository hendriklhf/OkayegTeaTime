using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct DotNetFiddleResult : IEquatable<DotNetFiddleResult>
{
    [JsonPropertyName("ConsoleOutput")]
    public required string ConsoleOutput { get; init; }

    public static DotNetFiddleResult Empty => new()
    {
        ConsoleOutput = null!
    };

    public DotNetFiddleResult()
    {
    }

    public bool Equals(DotNetFiddleResult other) => ConsoleOutput == other.ConsoleOutput;

    public override bool Equals(object? obj) => obj is DotNetFiddleResult other && Equals(other);

    public override int GetHashCode() => ConsoleOutput.GetHashCode();

    public static bool operator ==(DotNetFiddleResult? left, DotNetFiddleResult? right) => Equals(left, right);

    public static bool operator !=(DotNetFiddleResult? left, DotNetFiddleResult? right) => !(left == right);
}
