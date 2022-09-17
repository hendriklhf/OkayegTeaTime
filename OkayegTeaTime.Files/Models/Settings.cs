#nullable disable

namespace OkayegTeaTime.Files.Models;

public sealed class Settings
{
    public TwitchSettings Twitch { get; set; }

    public DiscordSettings Discord { get; set; }

    public string[] OfflineChatEmotes { get; set; }

    public SpotifySettings Spotify { get; set; }

    public UserLists UserLists { get; set; }

    public string RepositoryUrl { get; set; }

    public string OfflineChatChannel { get; set; }

    public DbConnection DatabaseConnection { get; set; }

    public string OpenWeatherMapApiKey { get; set; }
}
