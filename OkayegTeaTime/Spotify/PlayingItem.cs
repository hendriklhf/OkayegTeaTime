#nullable disable

namespace OkayegTeaTime.Spotify;

public abstract class PlayingItem
{
    public string Title { get; protected set; }

    public string[] Artists { get; protected set; }

    public string Uri { get; protected set; }

    public string Message { get; protected set; }

    public bool IsLocal { get; protected set; }
}
