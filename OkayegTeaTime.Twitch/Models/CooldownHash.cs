using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct CooldownHash : IEquatable<CooldownHash>
{
    private readonly int _hash;

    public CooldownHash(long userId, CommandType type)
    {
        _hash = HashCode.Combine(userId, type);
    }

    public CooldownHash(long userId)
    {
        _hash = userId.GetHashCode();
    }

    public bool Equals(CooldownHash other)
    {
        return other._hash == _hash;
    }

    public override bool Equals(object? obj)
    {
        return obj is CooldownHash other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public static bool operator ==(CooldownHash left, CooldownHash right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CooldownHash left, CooldownHash right)
    {
        return !(left == right);
    }

    public static implicit operator int(CooldownHash hash)
    {
        return hash._hash;
    }
}
