using System;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct SmartSplit : IDisposable, IEquatable<SmartSplit>
{
    public ReadOnlyMemory<char> this[int index] => _message[_ranges[index]];

    public ReadOnlySpan<ReadOnlyMemory<char>> Splits => GetSplits();

    public int Length { get; }

    private readonly ReadOnlyMemory<char> _message;
    private readonly RentedArray<Range> _ranges = new(255);

    public static SmartSplit Empty => new();

    public SmartSplit()
    {
        _message = ReadOnlyMemory<char>.Empty;
        _ranges = RentedArray<Range>.Empty;
        Length = 0;
    }

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        Length = message.Span.Split(_ranges, ' ');
    }

    private ReadOnlyMemory<char>[] GetSplits()
    {
        ReadOnlyMemory<char>[] splits = new ReadOnlyMemory<char>[Length];
        for (int i = 0; i < Length; i++)
        {
            splits[i] = _message[_ranges[i]];
        }

        return splits;
    }

    public void Dispose() => _ranges.Dispose();

    public bool Equals(SmartSplit other) => Length == other.Length && _message.Equals(other._message) && _ranges.Equals(other._ranges);

    public override bool Equals(object? obj) => obj is SmartSplit other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_message, _ranges, Length);

    public static bool operator ==(SmartSplit left, SmartSplit right) => left.Equals(right);

    public static bool operator !=(SmartSplit left, SmartSplit right) => !(left == right);
}
