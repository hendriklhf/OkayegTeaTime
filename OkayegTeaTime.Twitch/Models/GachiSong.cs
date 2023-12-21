namespace OkayegTeaTime.Twitch.Models;

public sealed record GachiSong
{
    public required string Title { get; init; }

    public required string Url { get; init; }
}
