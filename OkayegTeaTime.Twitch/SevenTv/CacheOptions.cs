using System;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Twitch.SevenTv;

public sealed class CacheOptions : IEquatable<CacheOptions>
{
    public TimeSpan GlobalEmotesCacheTime { get; set; } = TimeSpan.FromDays(1);

    public TimeSpan ChannelEmotesCacheTime { get; set; } = TimeSpan.FromHours(1);

    public bool Equals(CacheOptions? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is CacheOptions other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(CacheOptions? left, CacheOptions? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CacheOptions? left, CacheOptions? right)
    {
        return !(left == right);
    }
}
