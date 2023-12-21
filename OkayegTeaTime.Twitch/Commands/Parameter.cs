using System;

namespace OkayegTeaTime.Twitch.Commands;

public readonly struct Parameter(string name, string description) : IEquatable<Parameter>
{
    public string Name { get; } = name;

    public string Description { get; } = description;

    public bool Equals(Parameter other) => Name == other.Name && Description == other.Description;

    public override bool Equals(object? obj) => obj is Parameter other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Name, Description);

    public static bool operator ==(Parameter left, Parameter right) => left.Equals(right);

    public static bool operator !=(Parameter left, Parameter right) => !(left == right);
}
