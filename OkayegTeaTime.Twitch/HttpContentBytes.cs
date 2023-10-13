using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HLE.Memory;

namespace OkayegTeaTime.Twitch;

public readonly struct HttpContentBytes : IEquatable<HttpContentBytes>, IDisposable
{
    public int Length { get; }

    private readonly RentedArray<byte> _bytes = RentedArray<byte>.Empty;

    public static HttpContentBytes Empty => new();

    public HttpContentBytes()
    {
    }

    private HttpContentBytes(RentedArray<byte> bytes, int length)
    {
        _bytes = bytes;
        Length = length;
    }

    public ReadOnlySpan<byte> AsSpan() => _bytes.Span[..Length];

    public ReadOnlyMemory<byte> AsMemory() => _bytes.Memory[..Length];

    private static readonly FieldInfo _underlyingArrayField = typeof(RentedArray<byte>).GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance)!;

    public static async ValueTask<HttpContentBytes> CreateAsync(HttpResponseMessage httpResponse)
    {
        int contentLength = (int)(httpResponse.Content.Headers.ContentLength ?? 0);
        if (contentLength == 0)
        {
            return Empty;
        }

        using RentedArray<byte> buffer = new(contentLength);
        byte[] underlyingArray = GetUnderlyingArray(buffer);
        MemoryStream copyDestination = new(underlyingArray);

        await httpResponse.Content.LoadIntoBufferAsync();
        await httpResponse.Content.CopyToAsync(copyDestination);
        return new(buffer, contentLength);
    }

    // TODO: fix with next HLE version
    private static byte[] GetUnderlyingArray(RentedArray<byte> rentedArray)
    {
        return (byte[])_underlyingArrayField.GetValue(rentedArray)!;
    }

    public bool Equals(HttpContentBytes other)
    {
        return Length == other.Length && _bytes == other._bytes;
    }

    public override bool Equals(object? obj)
    {
        return obj is HttpContentBytes other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_bytes, Length);
    }

    public static bool operator ==(HttpContentBytes left, HttpContentBytes right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HttpContentBytes left, HttpContentBytes right)
    {
        return !(left == right);
    }

    public void Dispose()
    {
        _bytes.Dispose();
    }
}
