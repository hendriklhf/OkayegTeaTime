using System;

namespace OkayegTeaTime.Spotify;

public sealed class SpotifyException(string message) : Exception
{
    public override string Message { get; } = message;
}
