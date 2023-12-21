using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct AliasHash(ReadOnlyMemory<char> alias) : IEquatable<AliasHash>
{
    private readonly ReadOnlyMemory<char> _alias = alias;

    public bool Equals(AliasHash other) => _alias.Span.SequenceEqual(other._alias.Span);

    public override bool Equals(object? obj) => obj is AliasHash other && Equals(other);

    public override int GetHashCode() => string.GetHashCode(_alias.Span, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(AliasHash left, AliasHash right) => left.Equals(right);

    public static bool operator !=(AliasHash left, AliasHash right) => !(left == right);
}
