#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class SpotifySettings
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string ChatPlaylistId { get; set; }

    public long[] ChatPlaylistUsers { get; set; }
}
