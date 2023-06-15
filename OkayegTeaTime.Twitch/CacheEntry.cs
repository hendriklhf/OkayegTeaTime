using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace OkayegTeaTime.Twitch;

internal readonly struct CacheEntry<T> : IEquatable<CacheEntry<T>>
{
    public T? Value { get; }

    internal readonly DateTime _timeOfRequest;

    public static CacheEntry<T> Empty => new();

    public CacheEntry()
    {
        Value = default;
        _timeOfRequest = default;
    }

    public CacheEntry(T value)
    {
        Value = value;
        _timeOfRequest = DateTime.UtcNow;
    }

    [Pure]
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsValid(TimeSpan cacheTime)
    {
        return this != default && _timeOfRequest + cacheTime > DateTime.UtcNow;
    }

    [Pure]
    public bool Equals(CacheEntry<T> other)
    {
        return Value?.Equals(other.Value) == true && _timeOfRequest == other._timeOfRequest;
    }

    [Pure]
    public override bool Equals(object? obj)
    {
        return obj is CacheEntry<T> other && Equals(other);
    }

    [Pure]
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, _timeOfRequest);
    }

    [Pure]
    public static bool operator ==(CacheEntry<T> left, CacheEntry<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CacheEntry<T> left, CacheEntry<T> right)
    {
        return !(left == right);
    }
}
