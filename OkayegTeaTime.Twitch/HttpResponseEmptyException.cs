using System;

namespace OkayegTeaTime.Twitch;

public sealed class HttpResponseEmptyException : Exception
{
    public HttpResponseEmptyException() : base("The HTTP response contains an empty body.")
    {
    }
}
