using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct CooldownHash(long userId, CommandType type = 0) : IEquatable<CooldownHash>
{
    private readonly long _userId = userId;
    private readonly CommandType _commandType = type;

    public bool Equals(CooldownHash other) => _userId == other._userId && _commandType == other._commandType;

    public override bool Equals(object? obj) => obj is CooldownHash other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_userId, _commandType);

    public static bool operator ==(CooldownHash left, CooldownHash right) => left.Equals(right);

    public static bool operator !=(CooldownHash left, CooldownHash right) => !(left == right);
}
