using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct AliasHash : IEquatable<AliasHash>
{
    private readonly int _hash;

    public AliasHash(ReadOnlySpan<char> alias)
    {
        _hash = string.GetHashCode(alias, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(AliasHash other)
    {
        return other._hash == _hash;
    }

    public override bool Equals(object? obj)
    {
        return obj is AliasHash other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public static bool operator ==(AliasHash left, AliasHash right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(AliasHash left, AliasHash right)
    {
        return !(left == right);
    }

    public static implicit operator int(AliasHash hash)
    {
        return hash._hash;
    }
}
