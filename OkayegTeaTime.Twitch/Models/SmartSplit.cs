using System;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public struct SmartSplit : IDisposable, IEquatable<SmartSplit>
{
    public readonly ReadOnlyMemory<char> this[int index] => _message[_ranges[index]];

    public int Length { get; }

    private readonly ReadOnlyMemory<char> _message;
    private RentedArray<Range> _ranges = ArrayPool<Range>.Shared.RentAsRentedArray(255);
    private RentedArray<ReadOnlyMemory<char>> _splits = [];

    public static SmartSplit Empty => new();

    public SmartSplit()
    {
        _message = ReadOnlyMemory<char>.Empty;
        _ranges = [];
        Length = 0;
    }

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        Length = message.Span.Split(_ranges.AsSpan(), ' ');
    }

    public ReadOnlySpan<ReadOnlyMemory<char>> AsSpan() => GetSplits().AsSpan();

    private RentedArray<ReadOnlyMemory<char>> GetSplits()
    {
        if (_splits != RentedArray<ReadOnlyMemory<char>>.Empty)
        {
            return _splits;
        }

        _splits = ArrayPool<ReadOnlyMemory<char>>.Shared.RentAsRentedArray(Length);
        for (int i = 0; i < Length; i++)
        {
            _splits[i] = _message[_ranges[i]];
        }

        return _splits;
    }

    public void Dispose()
    {
        _ranges.Dispose();
        _splits.Dispose();
    }

    public readonly bool Equals(SmartSplit other) => Length == other.Length && _message.Equals(other._message) && _ranges.Equals(other._ranges);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is SmartSplit other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => HashCode.Combine(_message, _ranges, Length);

    public static bool operator ==(SmartSplit left, SmartSplit right) => left.Equals(right);

    public static bool operator !=(SmartSplit left, SmartSplit right) => !(left == right);
}
