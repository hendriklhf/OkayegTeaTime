using System;
using System.Threading.Tasks;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct PeriodicAction(Func<ValueTask> action, TimeSpan interval) : IEquatable<PeriodicAction>
{
    public Func<ValueTask> Action { get; } = action;

    public TimeSpan Interval { get; } = interval;

    public bool Equals(PeriodicAction other)
    {
        return Interval == other.Interval && Action.Equals(other.Action);
    }

    public override bool Equals(object? obj)
    {
        return obj is PeriodicAction other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Action, Interval);
    }

    public static bool operator ==(PeriodicAction left, PeriodicAction right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PeriodicAction left, PeriodicAction right)
    {
        return !left.Equals(right);
    }
}
