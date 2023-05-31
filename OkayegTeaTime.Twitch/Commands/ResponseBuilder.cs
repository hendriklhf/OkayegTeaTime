using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HLE.Memory;
using HLE.Strings;
using JetBrains.Annotations;

namespace OkayegTeaTime.Twitch.Commands;

[DebuggerDisplay("{ToString()}")]
public sealed class ResponseBuilder : IEquatable<ResponseBuilder>
{
    public ref char this[int index] => ref BufferSpan[index];

    public ref char this[Index index] => ref BufferSpan[index];

    public Span<char> this[Range range] => BufferSpan[range];

    public int Length { get; private set; }

    public int Capacity => _buffer.Length;

    public Span<char> BufferSpan => _buffer;

    public Memory<char> BufferMemory => _buffer;

    public ReadOnlySpan<char> WrittenSpan => BufferSpan[..Length];

    public ReadOnlyMemory<char> WrittenMemory => BufferMemory[..Length];

    public Span<char> FreeBufferSpan => BufferSpan[Length..];

    public Memory<char> FreeBufferMemory => BufferMemory[Length..];

    public int FreeBufferSize => _buffer.Length - Length;

    private char[] _buffer = Array.Empty<char>();

    private const int _minimumGrowth = 100;

    public static PoolBufferStringBuilder Empty => new();

    public ResponseBuilder()
    {
    }

    public ResponseBuilder(int initialBufferSize)
    {
        _buffer = ArrayPool<char>.Shared.Rent(initialBufferSize);
    }

    private void GrowBuffer(int size = _minimumGrowth)
    {
        if (size < _minimumGrowth)
        {
            size = _minimumGrowth;
        }

        char[] newBuffer = ArrayPool<char>.Shared.Rent(_buffer.Length + size);
        WrittenSpan.CopyTo(newBuffer);
        ArrayPool<char>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }

    public void Dispose()
    {
        if (!ReferenceEquals(_buffer, Array.Empty<char>()))
        {
            ArrayPool<char>.Shared.Return(_buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int length)
    {
        Length += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(scoped ReadOnlySpan<char> span)
    {
        if (FreeBufferSize < span.Length)
        {
            GrowBuffer(span.Length);
        }

        span.CopyTo(FreeBufferSpan);
        Length += span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        if (FreeBufferSize <= 0)
        {
            GrowBuffer();
        }

        _buffer[Length++] = c;
    }

    public void Append(byte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(sbyte value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(short value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(ushort value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(int value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(uint value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(long value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(ulong value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(float value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(double value, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!value.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(value, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append<TSpanFormattable, TFormatProvider>(TSpanFormattable spanFormattable, ReadOnlySpan<char> format = default, TFormatProvider? formatProvider = default)
        where TSpanFormattable : ISpanFormattable where TFormatProvider : IFormatProvider
    {
        if (!spanFormattable.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(spanFormattable, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(DateTime dateTime, [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!dateTime.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(dateTime, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Append(TimeSpan timeSpan, [StringSyntax(StringSyntaxAttribute.TimeSpanFormat)] ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
    {
        if (!timeSpan.TryFormat(FreeBufferSpan, out int charsWritten, format, formatProvider))
        {
            GrowBuffer();
            Append(timeSpan, format, formatProvider);
        }

        Advance(charsWritten);
    }

    public void Remove(int index, int length = 1)
    {
        BufferSpan[(index + length)..Length].CopyTo(BufferSpan[index..]);
        Length -= length;
    }

    public void Clear()
    {
        Length = 0;
    }

    [Pure]
    public override string ToString()
    {
        return new(WrittenSpan);
    }

    [Pure]
    public char[] ToCharArray()
    {
        return WrittenSpan.ToArray();
    }

    public void CopyTo(char[] destination, int offset = 0)
    {
        CopyTo(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(destination), offset));
    }

    public void CopyTo(Memory<char> destination)
    {
        CopyTo(ref MemoryMarshal.GetReference(destination.Span));
    }

    public void CopyTo(Span<char> destination)
    {
        CopyTo(ref MemoryMarshal.GetReference(destination));
    }

    public unsafe void CopyTo(ref char destination)
    {
        CopyTo((char*)Unsafe.AsPointer(ref destination));
    }

    public unsafe void CopyTo(char* destination)
    {
        char* source = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(_buffer));
        Unsafe.CopyBlock(destination, source, (uint)(Length * sizeof(char)));
    }

    [Pure]
    public bool Equals(ResponseBuilder builder, StringComparison comparisonType)
    {
        return WrittenSpan.Equals(builder.WrittenSpan, comparisonType);
    }

    [Pure]
    public bool Equals(ReadOnlySpan<char> str, StringComparison comparisonType)
    {
        return WrittenSpan.Equals(str, comparisonType);
    }

    public bool Equals(ResponseBuilder? other)
    {
        return ReferenceEquals(_buffer, other?._buffer) && Length == other.Length;
    }

    public override bool Equals(object? obj)
    {
        return obj is ResponseBuilder other && Equals(other);
    }

    [Pure]
    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    [Pure]
    public int GetHashCode(StringComparison comparisonType)
    {
        return string.GetHashCode(WrittenSpan, comparisonType);
    }

    public static bool operator ==(ResponseBuilder left, ResponseBuilder right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ResponseBuilder left, ResponseBuilder right)
    {
        return !(left == right);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1)
    {
        Append(arg0);
        Append(arg1);
    }

    public void Append(char arg0, char arg1)
    {
        Append(arg0);
        Append(arg1);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
    }

    public void Append(char arg0, char arg1, char arg2)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2, scoped ReadOnlySpan<char> arg3)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
    }

    public void Append(char arg0, char arg1, char arg2, char arg3)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2, scoped ReadOnlySpan<char> arg3, scoped ReadOnlySpan<char> arg4)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
    }

    public void Append(char arg0, char arg1, char arg2, char arg3, char arg4)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2, scoped ReadOnlySpan<char> arg3, scoped ReadOnlySpan<char> arg4, scoped ReadOnlySpan<char> arg5)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
    }

    public void Append(char arg0, char arg1, char arg2, char arg3, char arg4, char arg5)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2, scoped ReadOnlySpan<char> arg3, scoped ReadOnlySpan<char> arg4, scoped ReadOnlySpan<char> arg5, scoped ReadOnlySpan<char> arg6)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
        Append(arg6);
    }

    public void Append(char arg0, char arg1, char arg2, char arg3, char arg4, char arg5, char arg6)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
        Append(arg6);
    }

    public void Append(scoped ReadOnlySpan<char> arg0, scoped ReadOnlySpan<char> arg1, scoped ReadOnlySpan<char> arg2, scoped ReadOnlySpan<char> arg3, scoped ReadOnlySpan<char> arg4, scoped ReadOnlySpan<char> arg5, scoped ReadOnlySpan<char> arg6,
        scoped ReadOnlySpan<char> arg7)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
        Append(arg6);
        Append(arg7);
    }

    public void Append(char arg0, char arg1, char arg2, char arg3, char arg4, char arg5, char arg6, char arg7)
    {
        Append(arg0);
        Append(arg1);
        Append(arg2);
        Append(arg3);
        Append(arg4);
        Append(arg5);
        Append(arg6);
        Append(arg7);
    }
}
