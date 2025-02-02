using System;
using System.Collections.Immutable;
using System.Net;
using System.Text;

namespace OkayegTeaTime.Twitch;

public sealed class HttpRequestFailedException(int statusCode, ReadOnlySpan<byte> responseBytes)
    : Exception($"The request failed with code {statusCode} and delivered: {Encoding.UTF8.GetString(responseBytes)}")
{
    public HttpStatusCode HttpStatusCode { get; } = (HttpStatusCode)statusCode;

    public ImmutableArray<byte> HttpResponseContent { get; } = [.. responseBytes];

    public HttpRequestFailedException(HttpStatusCode statusCode, ReadOnlySpan<byte> responseBytes) : this((int)statusCode, responseBytes)
    {
    }
}
