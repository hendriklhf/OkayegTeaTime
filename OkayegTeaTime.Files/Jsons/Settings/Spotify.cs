#nullable disable

namespace OkayegTeaTime.Files.Jsons.Settings;

public class Spotify
{
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

#if RELEASE
    public string ChatPlaylistId { get; set; }

    public long[] ChatPlaylistUsers { get; set; }
#endif
}
