using System;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Twitch.Helix;

public sealed class CacheOptions : IEquatable<CacheOptions>
{
    public TimeSpan UserCacheTime { get; set; } = TimeSpan.FromDays(1);

    public TimeSpan StreamCacheTime { get; set; } = TimeSpan.FromMinutes(10);

    public TimeSpan GlobalEmotesCacheTime { get; set; } = TimeSpan.FromDays(1);

    public TimeSpan ChannelEmotesCacheTime { get; set; } = TimeSpan.FromDays(1);

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
