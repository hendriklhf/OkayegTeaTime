using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models.Responses;

public readonly struct Sets : IEquatable<Sets>
{
    [JsonPropertyName("3")]
    public required GlobalSet GlobalSet { get; init; }

    public bool Equals(Sets other) => GlobalSet.Equals(other.GlobalSet);

    public override bool Equals(object? obj) => obj is Sets other && Equals(other);

    public override int GetHashCode() => GlobalSet.GetHashCode();

    public static bool operator ==(Sets left, Sets right) => left.Equals(right);

    public static bool operator !=(Sets left, Sets right) => !left.Equals(right);
}
