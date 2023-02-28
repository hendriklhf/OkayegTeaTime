using System;
using System.Buffers;
using HLE;
using HLE.Memory;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct SmartSplit : IDisposable
{
    public ReadOnlySpan<char> this[int index] => _message.Span[_ranges[index]];

    public ReadOnlySpan<ReadOnlyMemory<char>> Splits => GetSplits();

    public int Length => _length;

    private readonly ReadOnlyMemory<char> _message;
    private readonly Range[] _ranges = ArrayPool<Range>.Shared.Rent(255);
    private readonly int _length;

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        _length = message.Span.GetRangesOfSplit(' ', _ranges);
    }

    public RentedArray<ReadOnlyMemory<char>> GetSplits()
    {
        ReadOnlyMemory<char>[] splits = ArrayPool<ReadOnlyMemory<char>>.Shared.Rent(_length);
        for (int i = 0; i < _length; i++)
        {
            splits[i] = _message[_ranges[i]];
        }

        return splits;
    }

    public void Dispose()
    {
        ArrayPool<Range>.Shared.Return(_ranges);
    }
}
