using System;
using System.Net;
using System.Text;

namespace OkayegTeaTime.Twitch.Api;

public sealed class HttpRequestFailedException : Exception
{
    public byte[] HttpResponseContent { get; }

    public HttpRequestFailedException(HttpStatusCode statusCode, ReadOnlySpan<byte> responseBytes) : this((int)statusCode, responseBytes)
    {
    }

    public HttpRequestFailedException(int statusCode, ReadOnlySpan<byte> responseBytes)
        : base($"The request failed with code {statusCode} and delivered: {Encoding.UTF8.GetString(responseBytes)}")
    {
        HttpResponseContent = responseBytes.ToArray();
    }
}
