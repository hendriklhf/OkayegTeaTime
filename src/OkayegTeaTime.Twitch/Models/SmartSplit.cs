using System;
using System.Threading;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public struct SmartSplit : IDisposable, IEquatable<SmartSplit>
{
    public readonly ReadOnlyMemory<char> this[int index] => _message[GetRanges()[index]];

    public int Length { get; }

    private readonly ReadOnlyMemory<char> _message;
    private Range[]? _ranges;
    private ReadOnlyMemory<char>[]? _splits;

    public static SmartSplit Empty => default;

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        _ranges = ArrayPool<Range>.Shared.Rent(255);
        Length = message.Span.Split(_ranges.AsSpan(), ' ');
    }

    public ReadOnlySpan<ReadOnlyMemory<char>> AsSpan() => GetSplits().AsSpan(..Length);

    private readonly Range[] GetRanges()
    {
        Range[]? ranges = _ranges;
        if (ranges is null)
        {
            ThrowHelpers.ThrowObjectDisposedException<SmartSplit>();
        }

        return ranges;
    }

    private ReadOnlyMemory<char>[] GetSplits()
    {
        if (_splits is not null)
        {
            return _splits;
        }

        ReadOnlySpan<Range> ranges = GetRanges();

        _splits = ArrayPool<ReadOnlyMemory<char>>.Shared.Rent(Length);
        for (int i = 0; i < Length; i++)
        {
            _splits[i] = _message[ranges[i]];
        }

        return _splits;
    }

    public void Dispose()
    {
        Range[]? ranges = Interlocked.Exchange(ref _ranges, null);
        if (ranges is not null)
        {
            ArrayPool<Range>.Shared.Return(ranges);
        }

        ReadOnlyMemory<char>[]? splits = Interlocked.Exchange(ref _splits, null);
        if (splits is not null)
        {
            ArrayPool<ReadOnlyMemory<char>>.Shared.Return(splits);
        }
    }

    public readonly bool Equals(SmartSplit other) => Length == other.Length && _message.Equals(other._message) && _ranges == other._ranges;

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is SmartSplit other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => HashCode.Combine(_message, _ranges, Length);

    public static bool operator ==(SmartSplit left, SmartSplit right) => left.Equals(right);

    public static bool operator !=(SmartSplit left, SmartSplit right) => !(left == right);
}
