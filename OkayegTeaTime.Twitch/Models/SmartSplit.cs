using System;
using System.Buffers;
using HLE.Memory;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct SmartSplit : IDisposable
{
    public ReadOnlyMemory<char> this[int index] => _message[_ranges[index]];

    public int Length => _length;

    private readonly ReadOnlyMemory<char> _message;
    private readonly RentedArray<Range> _ranges = ArrayPool<Range>.Shared.Rent(255);
    private readonly int _length;

    public SmartSplit(ReadOnlyMemory<char> message)
    {
        _message = message;
        _length = message.Span.GetRangesOfSplit(' ', _ranges);
    }

    public RentedArray<ReadOnlyMemory<char>> GetSplits()
    {
        RentedArray<ReadOnlyMemory<char>> splits = ArrayPool<ReadOnlyMemory<char>>.Shared.Rent(_length);
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
