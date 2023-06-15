using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct CooldownHash : IEquatable<CooldownHash>
{
    private readonly long _userId;
    private readonly CommandType _commandType;

    public CooldownHash(long userId, CommandType type)
    {
        _userId = userId;
        _commandType = type;
    }

    public CooldownHash(long userId)
    {
        _userId = userId;
        _commandType = 0;
    }

    public bool Equals(CooldownHash other)
    {
        return _userId == other._userId && _commandType == other._commandType;
    }

    public override bool Equals(object? obj)
    {
        return obj is CooldownHash other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_userId, _commandType);
    }

    public static bool operator ==(CooldownHash left, CooldownHash right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CooldownHash left, CooldownHash right)
    {
        return !(left == right);
    }
}
