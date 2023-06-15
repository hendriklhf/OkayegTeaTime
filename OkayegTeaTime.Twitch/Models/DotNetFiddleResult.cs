using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct DotNetFiddleResult : IEquatable<DotNetFiddleResult>
{
    [JsonPropertyName("ConsoleOutput")]
    public required string ConsoleOutput { get; init; } = string.Empty;

    public static DotNetFiddleResult Empty => new()
    {
        ConsoleOutput = string.Empty
    };

    public DotNetFiddleResult()
    {
    }

    public bool Equals(DotNetFiddleResult other)
    {
        return ConsoleOutput == other.ConsoleOutput;
    }

    public override bool Equals(object? obj)
    {
        return obj is DotNetFiddleResult other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ConsoleOutput.GetHashCode();
    }

    public static bool operator ==(DotNetFiddleResult? left, DotNetFiddleResult? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DotNetFiddleResult? left, DotNetFiddleResult? right)
    {
        return !(left == right);
    }
}
