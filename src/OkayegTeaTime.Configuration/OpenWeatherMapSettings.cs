using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Configuration;

public sealed class OpenWeatherMapSettings : IEquatable<OpenWeatherMapSettings>
{
    [RegularExpression("^[a-z0-9]{32}$")]
    public required string ApiKey { get; init; }

    public bool Equals(OpenWeatherMapSettings? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is OpenWeatherMapSettings other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(OpenWeatherMapSettings? left, OpenWeatherMapSettings? right) => Equals(left, right);

    public static bool operator !=(OpenWeatherMapSettings? left, OpenWeatherMapSettings? right) => !(left == right);
}
