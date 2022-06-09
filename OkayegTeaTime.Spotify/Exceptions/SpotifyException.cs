using System;

namespace OkayegTeaTime.Spotify.Exceptions;

public class SpotifyException : Exception
{
    public override string Message { get; }

    public SpotifyException(string message)
    {
        Message = message;
    }
}
