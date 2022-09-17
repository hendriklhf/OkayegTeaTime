#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class SpotifySettings
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

#if RELEASE
    public string ChatPlaylistId { get; set; }

    public long[] ChatPlaylistUsers { get; set; }
#endif
}
