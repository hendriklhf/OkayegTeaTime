using System;
using HLE.Memory;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct SmartSplit : IDisposable, IEquatable<SmartSplit>
{
    public ReadOnlyMemory<char> this[int index] => _message[_ranges[index]];

    public int Length => _length;

    private readonly ReadOnlyMemory<char> _message;
    private readonly RentedArray<Range> _ranges = new(255);
    private readonly int _length;

    public static SmartSplit Empty => new();

    public SmartSplit()
    {
        _message = ReadOnlyMemory<char>.Empty;
        _ranges = RentedArray<Range>.Empty;
        _length = 0;
    }

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        _length = message.Span.GetRangesOfSplit(' ', _ranges);
    }

    public RentedArray<ReadOnlyMemory<char>> GetSplits()
    {
        RentedArray<ReadOnlyMemory<char>> splits = new(_length);
        for (int i = 0; i < _length; i++)
        {
            splits[i] = _message[_ranges[i]];
        }

        return splits;
    }

    public void Dispose()
    {
        _ranges.Dispose();
    }

    public bool Equals(SmartSplit other)
    {
        return _length == other._length && _message.Equals(other._message) && _ranges.Equals(other._ranges);
    }

    public override bool Equals(object? obj)
    {
        return obj is SmartSplit other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_message, _ranges, _length);
    }

    public static bool operator ==(SmartSplit left, SmartSplit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SmartSplit left, SmartSplit right)
    {
        return !(left == right);
    }
}
