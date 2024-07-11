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
#pragma warning disable S6354
        _timeOfRequest = DateTime.UtcNow;
#pragma warning restore S6354
    }

    [Pure]
    [MemberNotNullWhen(true, nameof(Value))]
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public bool IsValid(TimeSpan cacheTime) => this != default && _timeOfRequest + cacheTime > DateTime.UtcNow;

    [Pure]
    public bool Equals(CacheEntry<T> other) => Value?.Equals(other.Value) == true && _timeOfRequest == other._timeOfRequest;

    [Pure]
    public override bool Equals(object? obj) => obj is CacheEntry<T> other && Equals(other);

    [Pure]
    public override int GetHashCode() => HashCode.Combine(Value, _timeOfRequest);

    [Pure]
    public static bool operator ==(CacheEntry<T> left, CacheEntry<T> right) => left.Equals(right);

    public static bool operator !=(CacheEntry<T> left, CacheEntry<T> right) => !(left == right);
}
