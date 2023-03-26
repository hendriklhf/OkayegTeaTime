using System;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct EmoteEntry<T> : IEquatable<EmoteEntry<T>>
{
    public ReadOnlySpan<T> Emotes => _emotes;

    public DateTime TimeOfRequest { get; }

    private readonly T[]? _emotes;

    public static EmoteEntry<T> Empty => new();

    public EmoteEntry()
    {
        _emotes = null;
        TimeOfRequest = default;
    }

    public EmoteEntry(T[]? emotes)
    {
        _emotes = emotes;
        TimeOfRequest = DateTime.UtcNow;
    }

    public bool IsValid(TimeSpan cacheTime)
    {
        return TimeOfRequest + cacheTime > DateTime.UtcNow;
    }

    public bool Equals(EmoteEntry<T> other)
    {
        return ReferenceEquals(_emotes, other._emotes);
    }

    public override bool Equals(object? obj)
    {
        return obj is EmoteEntry<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_emotes, TimeOfRequest);
    }

    public static bool operator ==(EmoteEntry<T> left, EmoteEntry<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EmoteEntry<T> left, EmoteEntry<T> right)
    {
        return !(left == right);
    }
}
