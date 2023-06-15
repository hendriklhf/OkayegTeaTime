using System;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Ffz;

public sealed class CacheOptions : IEquatable<CacheOptions>
{
    public TimeSpan ChannelEmotesCacheTime { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan GlobalEmotesCacheTime { get; set; } = TimeSpan.FromDays(1);

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
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
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
