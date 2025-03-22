using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HLE.Memory;

namespace OkayegTeaTime.Twitch;

public struct HttpContentBytes : IEquatable<HttpContentBytes>, IDisposable
{
    public int Length { get; }

    private byte[]? _bytes = [];

    public static HttpContentBytes Empty => new();

    public HttpContentBytes()
    {
    }

    private HttpContentBytes(byte[] bytes, int length)
    {
        _bytes = bytes;
        Length = length;
    }

    public readonly ReadOnlySpan<byte> AsSpan()
    {
        if (_bytes is null)
        {
            throw new ObjectDisposedException(typeof(HttpContentBytes).FullName);
        }

        return _bytes.AsSpan(..Length);
    }

    public readonly ReadOnlyMemory<byte> AsMemory()
    {
        if (_bytes is null)
        {
            throw new ObjectDisposedException(typeof(HttpContentBytes).FullName);
        }

        return _bytes.AsMemory(..Length);
    }

    public static async ValueTask<HttpContentBytes> CreateAsync(HttpResponseMessage httpResponse)
    {
        int contentLength = (int)(httpResponse.Content.Headers.ContentLength ?? 0);
        if (contentLength == 0)
        {
            return Empty;
        }

        byte[] buffer = ArrayPool<byte>.Shared.Rent(contentLength);
        await using MemoryStream copyDestination = new(buffer);

        await httpResponse.Content.CopyToAsync(copyDestination);
        return new(buffer, contentLength);
    }

    public readonly bool Equals(HttpContentBytes other) => Length == other.Length && _bytes == other._bytes;

    public override readonly bool Equals(object? obj) => obj is HttpContentBytes other && Equals(other);

    public override readonly int GetHashCode() => HashCode.Combine(_bytes, Length);

    public static bool operator ==(HttpContentBytes left, HttpContentBytes right) => left.Equals(right);

    public static bool operator !=(HttpContentBytes left, HttpContentBytes right) => !(left == right);

    public void Dispose()
    {
        byte[]? bytes = Interlocked.Exchange(ref _bytes, null);
        if (bytes is null)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(bytes);
    }
}
