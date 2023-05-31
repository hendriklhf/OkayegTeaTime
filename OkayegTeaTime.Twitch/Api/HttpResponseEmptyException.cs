using System;

namespace OkayegTeaTime.Twitch.Api;

public sealed class HttpResponseEmptyException : Exception
{
    public HttpResponseEmptyException() : base("The HTTP response contains an empty body.")
    {
    }
}
