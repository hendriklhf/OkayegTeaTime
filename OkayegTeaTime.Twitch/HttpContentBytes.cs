using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HLE.Marshalling;
using HLE.Memory;

namespace OkayegTeaTime.Twitch;

public struct HttpContentBytes : IEquatable<HttpContentBytes>, IDisposable
{
    public int Length { get; }

    private RentedArray<byte> _bytes = [];

    public static HttpContentBytes Empty => new();

    public HttpContentBytes()
    {
    }

    private HttpContentBytes(RentedArray<byte> bytes, int length)
    {
        _bytes = bytes;
        Length = length;
    }

    public readonly ReadOnlySpan<byte> AsSpan() => _bytes.AsSpan(..Length);

    public readonly ReadOnlyMemory<byte> AsMemory() => _bytes.AsMemory(..Length);

    public static async ValueTask<HttpContentBytes> CreateAsync(HttpResponseMessage httpResponse)
    {
        int contentLength = (int)(httpResponse.Content.Headers.ContentLength ?? 0);
        if (contentLength == 0)
        {
            return Empty;
        }

        using RentedArray<byte> buffer = ArrayPool<byte>.Shared.RentAsRentedArray(contentLength);
        byte[] underlyingArray = RentedArrayMarshal<byte>.GetArray(buffer);
        await using MemoryStream copyDestination = new(underlyingArray);

        await httpResponse.Content.LoadIntoBufferAsync();
        await httpResponse.Content.CopyToAsync(copyDestination);
        return new(buffer, contentLength);
    }

    public readonly bool Equals(HttpContentBytes other) => Length == other.Length && _bytes == other._bytes;

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is HttpContentBytes other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => HashCode.Combine(_bytes, Length);

    public static bool operator ==(HttpContentBytes left, HttpContentBytes right) => left.Equals(right);

    public static bool operator !=(HttpContentBytes left, HttpContentBytes right) => !(left == right);

    public void Dispose() => _bytes.Dispose();
}
