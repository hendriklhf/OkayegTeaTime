using System;

namespace OkayegTeaTime.Spotify;

public sealed class SpotifyException : Exception
{
    public override string Message { get; }

    public SpotifyException(string message)
    {
        Message = message;
    }
}
